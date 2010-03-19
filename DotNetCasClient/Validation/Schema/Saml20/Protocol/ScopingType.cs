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
    [XmlRoot("Scoping", Namespace="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable=false)]
    public class ScopingType {
        [XmlElement("IDPList")]
        public IdpListType IdpList
        {
            get;
            set;
        }

        [XmlElement("RequesterID", DataType="anyURI")]
        public string[] RequesterId
        {
            get;
            set;
        }

        [XmlAttribute(DataType="nonNegativeInteger")]
        public string ProxyCount
        {
            get;
            set;
        }
    }
}