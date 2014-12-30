using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;

namespace AmazonWebServicesHelper.IAM
{
    public class IamPolicy
    {
        public IamPolicy(IamPolicyOrigin origin, string name, string json)
        {
            Json = System.Net.WebUtility.UrlDecode(json);
            Origin = origin;
            OriginName = name;

            Statements = new List<IamPolicyStatement>();


            dynamic parsed = JObject.Parse(Json);
            PolicyVersion = DateTime.Parse(parsed.Version.ToString());

            Statements = new List<IamPolicyStatement>();
            foreach (dynamic state in parsed.Statement)
            {
                Statements.Add(new IamPolicyStatement(state));
            }
        }

        public string Json { get; set; }
        public IamPolicyOrigin Origin { get; set; }
        public string OriginName { get; set; }
        public DateTime PolicyVersion { get; set; }
        public List<IamPolicyStatement> Statements { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Policy from " + Origin.ToString() + " " + OriginName + " [" + PolicyVersion.Date.ToShortDateString() + "]:");
            foreach (IamPolicyStatement statement in Statements)
            {
                using (StringReader reader = new StringReader(statement.ToString()))
                {
                    while (reader.Peek() != -1)
                    {
                        builder.AppendLine("\t" + reader.ReadLine());
                    }
                }
            }

            return builder.ToString();
        }
    }
}
