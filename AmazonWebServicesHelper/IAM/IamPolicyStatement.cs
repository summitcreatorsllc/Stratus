using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AmazonWebServicesHelper.IAM
{
    public class IamPolicyStatement
    {
        public IamPolicyStatement(dynamic state)
        {
            if (state.Effect.ToString() == "Allow")
            {
                Rule = IamStatementRule.Allow;
            }
            else
            {
                Rule = IamStatementRule.Deny;
            }
            Resource = new Common.ArnResource(state.Resource.ToString());

            Actions = new List<IamPolicyAction>();
            try
            {
                foreach (dynamic act in state.Action)
                {
                    Actions.Add(new IamPolicyAction(act.ToString()));
                }
            }
            catch
            {
                Actions.Add(new IamPolicyAction("*:*"));
            }
        }

        public List<IamPolicyAction> Actions { get; set; }
        public IamStatementRule Rule { get; set; }
        public Common.ArnResource Resource { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(Rule.ToString().ToUpper() + " ON " + Resource);
            foreach (IamPolicyAction act in Actions)
            {
                using (var reader = new StringReader(act.ToString()))
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
