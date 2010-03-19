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
    [XmlRoot("IDPList", Namespace="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable=false)]
    public class IdpListType {
        [XmlElement("IDPEntry")]
        public IdpEntryType[] IdpEntry
        {
            get;
            set;
        }

        [XmlElement(DataType="anyURI")]
        public string GetComplete
        {
            get;
            set;
        }
    }
}