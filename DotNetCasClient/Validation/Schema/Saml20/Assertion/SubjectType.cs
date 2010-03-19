using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Assertion
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
    [XmlRoot("Subject", Namespace="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable=false)]
    public class SubjectType {
        [XmlElement("BaseID", typeof(BaseIdAbstractType))]
        [XmlElement("EncryptedID", typeof(EncryptedElementType))]
        [XmlElement("NameID", typeof(NameIdType))]
        [XmlElement("SubjectConfirmation", typeof(SubjectConfirmationType))]
        public object[] Items
        {
            get;
            set;
        }
    }
}