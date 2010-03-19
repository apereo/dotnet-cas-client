using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.Saml20.Assertion;

namespace DotNetCasClient.Validation.Schema.Saml20.Protocol.Request
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:profiles:SSO:ecp")]
    [XmlRoot("Request", Namespace="urn:oasis:names:tc:SAML:2.0:profiles:SSO:ecp", IsNullable=false)]
    public class RequestType {
        [XmlElement(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
        public NameIdType Issuer
        {
            get;
            set;
        }

        [XmlElement("IDPList", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
        public IdpListType IdpList
        {
            get;
            set;
        }

        [XmlAttribute("mustUnderstand", Form=System.Xml.Schema.XmlSchemaForm.Qualified, Namespace="http://schemas.xmlsoap.org/soap/envelope/")]
        public bool MustUnderstand
        {
            get;
            set;
        }

        [XmlAttribute("actor", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://schemas.xmlsoap.org/soap/envelope/", DataType = "anyURI")]
        public string Actor
        {
            get;
            set;
        }

        [XmlAttribute]
        public string ProviderName
        {
            get;
            set;
        }

        [XmlAttribute]
        public bool IsPassive
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool IsPassiveSpecified
        {
            get;
            set;
        }
    }
}