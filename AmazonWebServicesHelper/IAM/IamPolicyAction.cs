using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonWebServicesHelper.IAM
{
    public class IamPolicyAction
    {
        public IamPolicyAction(string action)
        {
            ServiceName = action.Split(':')[0];
            ActionName = action.Split(':')[1];

            if (ActionName == "*") ActionName = "ALL";
            if (ServiceName == "*") ServiceName = "ALL";
        }

        public string ServiceName { get; set; }
        public string ActionName { get; set; }

        public override string ToString()
        {
            return ServiceName + " - " + ActionName;
        }
    }
}
