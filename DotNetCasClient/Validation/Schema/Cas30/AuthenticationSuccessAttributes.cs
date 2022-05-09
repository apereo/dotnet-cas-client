using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace DotNetCasClient.Validation.Schema.Cas30
{
    public class AuthenticationSuccessAttributes
    {
        internal AuthenticationSuccessAttributes() { }

        public static AuthenticationSuccessAttributes ParseResponse(string casResponse)
        {
            XmlDocument document = new XmlDocument();
            document.Load(new StringReader(casResponse));

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);
            nsmgr.AddNamespace("cas", "http://www.yale.edu/tp/cas");

            var entity = new AuthenticationSuccessAttributes()
            {
                Attributes = new Dictionary<string, IList<string>>()
            };
            var attributes = entity.Attributes;

            if (document.DocumentElement != null)
            {
                XmlNode xmlNode = document.DocumentElement.SelectSingleNode("cas:authenticationSuccess/cas:attributes", nsmgr);

                if (xmlNode != null)
                {
                    foreach (XmlNode node in xmlNode.ChildNodes)
                    {
                        string key = node.LocalName, value = node.InnerText;
                        if (!attributes.ContainsKey(key))
                        {
                            attributes.Add(key, new List<string>());
                        }
                        attributes[key].Add(value);
                    }
                }
            }

            return entity;
        }

        public IDictionary<string, IList<string>> Attributes;
    }
}
