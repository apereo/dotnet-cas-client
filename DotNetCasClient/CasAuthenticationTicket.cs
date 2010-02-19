using System;
using System.Collections.Generic;
using System.Text;
using DotNetCasClient.Security;

namespace DotNetCasClient.State
{
    [Serializable]
    public sealed class CasAuthenticationTicket
    {
        public string NetId { get; private set; }
        public string ServiceTicket { get; set; }
        public string OriginatingServiceName { get; set; }
        public string ClientHostAddress { get; set; }
        public IAssertion Assertion { get; set; }
        public DateTime ValidFromDate { get; set; }
        public DateTime ValidUntilDate { get; set; }

        public bool Expired
        {
            get
            {
                return (DateTime.Now.CompareTo(ValidUntilDate) > 0);
            }
        }

        public CasAuthenticationTicket()
        {
        }

        public CasAuthenticationTicket(string serviceTicket, string originatingServiceName, string clientHostAddress, IAssertion assertion)
        {
            CasAuthentication.Initialize();

            NetId = assertion.PrincipalName;
            ServiceTicket = serviceTicket;
            OriginatingServiceName = originatingServiceName;
            ClientHostAddress = clientHostAddress;
            Assertion = assertion;

            if (DateTime.MinValue.CompareTo(assertion.ValidFromDate) != 0)
            {
                ValidFromDate = assertion.ValidFromDate;
            }
            else
            {
                ValidFromDate = DateTime.Now;
            }

            DateTime localValidUntil = ValidFromDate.Add(CasAuthentication.FormsTimeout);            
            if (DateTime.MinValue.CompareTo(assertion.ValidUntilDate) != 0)
            {
                ValidUntilDate = localValidUntil.CompareTo(assertion.ValidUntilDate) < 0 ? localValidUntil : assertion.ValidUntilDate;
            }
            else
            {
                ValidUntilDate = localValidUntil;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("[{0}]{1}", ServiceTicket, Environment.NewLine);
            builder.AppendFormat("  NetID............. {0}{1}", NetId, Environment.NewLine);
            builder.AppendFormat("  Origin Service.... {0}{1}", OriginatingServiceName, Environment.NewLine);
            builder.AppendFormat("  Client Address.... {0}{1}", ClientHostAddress, Environment.NewLine);
            builder.AppendFormat("  Valid From........ {0}{1}", ValidFromDate, Environment.NewLine);
            builder.AppendFormat("  Valid Until....... {0}{1}{2}", ValidUntilDate, (Expired ? " (Expired!)" : string.Empty), Environment.NewLine);
            builder.AppendLine  ("  Assertion.........");
            builder.AppendFormat("    Principal....... {0}{1}", Assertion.PrincipalName, Environment.NewLine);
            builder.AppendFormat("    Valid From...... {0}{1}", Assertion.ValidFromDate, Environment.NewLine);
            builder.AppendFormat("    Valid Until..... {0}{1}", Assertion.ValidUntilDate, Environment.NewLine);
            builder.AppendLine  ("    Attributes......");
            foreach (KeyValuePair<string, IList<string>> attribute in Assertion.Attributes)
            {
                builder.AppendLine("      " + attribute.Key);
                foreach (string valuePart in attribute.Value)
                {
                    builder.AppendLine("        " + valuePart);
                }
            }
            return builder.ToString();
        }
    }
}


