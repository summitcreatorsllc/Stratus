using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using Amazon;
using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;
using AmazonWebServicesHelper.Exceptions;

namespace AmazonWebServicesHelper.IAM
{
    public class IamCredential
    {
        public IamCredential()
        {
            Region = "";
            AccessKey = "";
            SecretKey = "";
        }
        public IamCredential(string file)
        {
            Load(file);
        }

        #region "Credential Properties"

        private string username;
        public string Username
        {
            get
            {
                if (username == null) username = GetUsernameFromAccessKeyAndSecret(AccessKey, SecretKey);
                return username;
            }
            set
            {
                username = value;
            }
        }
        public string Region { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }

        #endregion

        #region "Serialization Methods"

        private void Load(string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    int ivLength = reader.ReadInt32();
                    int keyLength = reader.ReadInt32();
                    int dataLength = reader.ReadInt32();

                    byte[] iv = new byte[ivLength];
                    reader.Read(iv, 0, ivLength);

                    byte[] key = new byte[keyLength];
                    reader.Read(key, 0, keyLength);

                    byte[] data = new byte[dataLength];
                    reader.Read(data, 0, dataLength);

                    using (RijndaelManaged aes = new RijndaelManaged())
                    {
                        aes.Key = key;
                        aes.IV = iv;

                        using (var ms = new MemoryStream(data))
                        using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        using (var sr = new StreamReader(cs))
                        {
                            string reg = "";
                            try
                            {
                                reg = sr.ReadLine();
                            }
                            catch
                            {
                                throw new InvalidCredentialException("The credential file did not contain all the data necessary.");
                            }
                            try
                            {
                                Region = Amazon.RegionEndpoint.GetBySystemName(reg).SystemName;
                            }
                            catch
                            {
                                throw new InvalidRegionException("The region given in the credential file (\"" + reg + "\") is invalid.");
                            }
                            try
                            {
                                Username = sr.ReadLine();
                                AccessKey = sr.ReadLine();
                                SecretKey = sr.ReadLine();
                            }
                            catch
                            {
                                throw new InvalidCredentialException("The credential file did not contain all the data necessary.");
                            }
                        }
                    }
                }
            }
        }
        public bool Save(string file)
        {

            try
            {
                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    // We are using AES-256
                    aes.BlockSize = 128;
                    aes.KeySize = 256;

                    // Generate the IV and Key
                    aes.GenerateIV();
                    aes.GenerateKey();

                    byte[] data;

                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            using (var sw = new StreamWriter(cs))
                            {
                                sw.WriteLine(Region);
                                sw.WriteLine(Username);
                                sw.WriteLine(AccessKey);
                                sw.WriteLine(SecretKey);
                            }
                        }

                        data = ms.ToArray();
                    }



                    using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
                    {
                        using (BinaryWriter writer = new BinaryWriter(fs))
                        {
                            // Write three ints: the lengths of the IV, KEY, and DATA in bytes
                            writer.Write(aes.IV.Length);
                            writer.Write(aes.Key.Length);
                            writer.Write(data.Length);

                            // Write the IV and the KEY
                            writer.Write(aes.IV, 0, aes.IV.Length);
                            writer.Write(aes.Key, 0, aes.Key.Length);
                            writer.Write(data, 0, data.Length);
                        }
                    }

                    return true;

                }
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region "Dynamic Properties"

        private List<IamPolicy> policies;
        public List<IamPolicy> Policies
        {
            get
            {
                if (policies == null) policies = IamCredential.GetPolicies(this, Username);
                return policies;
            }
        }

        private List<Group> groups;
        public List<Group> Groups
        {
            get
            {
                if (groups == null) groups = LoadGroups();
                return groups;
            }
        }
        private List<Group> LoadGroups()
        {
            using (var client = new AmazonIdentityManagementServiceClient(AccessKey, SecretKey, RegionEndpoint.GetBySystemName(Region)))
            {
                var allGroupsResponse = client.ListGroupsForUser(new ListGroupsForUserRequest(Username));
                return allGroupsResponse.Groups;
            }
        }

        #endregion

        #region "Static Methods"

        public static List<IamPolicy> GetPolicies(IamCredential cred, string Username)
        {
            using (var client = new AmazonIdentityManagementServiceClient(cred.AccessKey, cred.SecretKey, RegionEndpoint.GetBySystemName(cred.Region)))
            {
                List<IamPolicy> policies = new List<IamPolicy>();

                var allGroupsResponse = client.ListGroupsForUser(new ListGroupsForUserRequest(Username));

                var groupPolicyNames = new List<GroupPolicyAssociation>();

                foreach (Group g in allGroupsResponse.Groups)
                {
                    var groupResponse = client.ListGroupPolicies(new ListGroupPoliciesRequest(g.GroupName));
                    foreach (string s in groupResponse.PolicyNames)
                    {
                        groupPolicyNames.Add(new GroupPolicyAssociation() { GroupName = g.GroupName, PolicyName = s});
                    }
                }

                var allUserResponse = client.ListUserPolicies(new ListUserPoliciesRequest(Username));
                List<string> userPolicyNames = allUserResponse.PolicyNames;

                foreach (GroupPolicyAssociation policy in groupPolicyNames)
                {
                    var gPolicyResponse = client.GetGroupPolicy(new GetGroupPolicyRequest(policy.GroupName, policy.PolicyName));
                    policies.Add(new IamPolicy(IamPolicyOrigin.Group, policy.GroupName, gPolicyResponse.PolicyDocument));
                }

                foreach (string policy in userPolicyNames)
                {
                    policies.Add(new IamPolicy(IamPolicyOrigin.User, Username, client.GetUserPolicy(new GetUserPolicyRequest(Username, policy)).PolicyDocument));
                }
                return policies;
            }
        }
        private class GroupPolicyAssociation
        {
            public string GroupName { get; set; }
            public string PolicyName { get; set; }
        }

        public static string GetUsernameFromAccessKeyAndSecret(string accessKey, string secret)
        {
            using (var client = new AmazonIdentityManagementServiceClient(accessKey, secret, RegionEndpoint.USWest1))
            {
                return client.GetUser().User.UserName;
            }
        }

        #endregion
    }
}
