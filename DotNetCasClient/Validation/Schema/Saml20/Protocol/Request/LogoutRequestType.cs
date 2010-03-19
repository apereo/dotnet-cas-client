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
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:protocol")]
    [XmlRoot("LogoutRequest", Namespace="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable=false)]
    public class LogoutRequestType : RequestAbstractType {
        [XmlElement("BaseID", typeof(BaseIdAbstractType), Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
        [XmlElement("EncryptedID", typeof(EncryptedElementType), Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
        [XmlElement("NameID", typeof(NameIdType), Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
        public object Item
        {
            get;
            set;
        }

        [XmlElement("SessionIndex")]
        public string[] SessionIndex
        {
            get;
            set;
        }

        [XmlAttribute]
        public string Reason
        {
            get;
            set;
        }

        [XmlAttribute]
        public DateTime NotOnOrAfter
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool NotOnOrAfterSpecified
        {
            get;
            set;
        }
    }
}