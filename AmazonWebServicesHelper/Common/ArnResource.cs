using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonWebServicesHelper.Common
{
    public class ArnResource
    {
        public ArnResource(string arn)
        {
            RawArn = arn;
            if (arn == "*")
            {
                IsWildcard = true;
                Partition = "*";
                ServiceNamespace = "*";
                Region = "*";
                Account = "*";
                Resource = "*";
                ResourceType = "*";
                return;
            }

            string[] splits = arn.Split(':');

            Partition = splits[1];
            ServiceNamespace = splits[2];
            Region = splits[3];
            if (Region == "" || Region == "*")
            {
                Region = "AllRegions";
            }
            Account = splits[4];
            
            if (splits.Length == 7)
            {
                ResourceType = splits[5];
                Resource = splits[6];
            }
            else if (arn.Contains('/'))
            {
                ResourceType = splits[5].Split('/')[0];
                Resource = splits[5].Split('/')[1];
            }
            else
            {
                Resource = splits[5];
            }

            if (Resource == "" || Resource == "*")
            {
                Resource = "AllResources";
            }
            
        }

        public string Partition { get; set; }
        public string ServiceNamespace { get; set; }
        public string Region { get; set; }
        public string Account { get; set; }
        public string Resource { get; set; }
        public string ResourceType { get; set; }
        public string RawArn { get; set; }
        public bool IsWildcard { get; set; }

        public override string ToString()
        {
            if (IsWildcard) return "*.*: AllResources";


            string tostr = Partition + "." + ServiceNamespace + ": ";
            if (ResourceType != null)
            {
                tostr += ResourceType + ":" + Resource;
            }
            else
            {
                tostr += Resource;
            }
            if (Account != "")
            {
                return tostr +" @ " + Account;
            }
            else
            {
                return tostr;
            }
        }
    }
}
