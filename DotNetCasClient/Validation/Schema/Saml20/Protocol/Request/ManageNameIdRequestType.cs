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
    [XmlRoot("ManageNameIDRequest", Namespace="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable=false)]
    public class ManageNameIdRequestType : RequestAbstractType {
        [XmlElement("EncryptedID", typeof(EncryptedElementType), Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
        [XmlElement("NameID", typeof(NameIdType), Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
        public object Item
        {
            get;
            set;
        }

        [XmlElement("NewEncryptedID", typeof(EncryptedElementType))]
        [XmlElement("NewID", typeof(string))]
        [XmlElement("Terminate", typeof(TerminateType))]
        public object Item1
        {
            get;
            set;
        }
    }
}