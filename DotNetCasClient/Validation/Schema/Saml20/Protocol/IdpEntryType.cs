using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Protocol
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:protocol")]
    [XmlRoot("IDPEntry", Namespace="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable=false)]
    public class IdpEntryType {
        [XmlAttribute("ProviderID", DataType="anyURI")]
        public string ProviderId
        {
            get;
            set;
        }

        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string Loc
        {
            get;
            set;
        }
    }
}