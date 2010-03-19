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
    [XmlRoot("SubjectConfirmation", Namespace="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable=false)]
    public class SubjectConfirmationType {
        [XmlElement("BaseID", typeof(BaseIdAbstractType))]
        [XmlElement("EncryptedID", typeof(EncryptedElementType))]
        [XmlElement("NameID", typeof(NameIdType))]
        public object Item
        {
            get;
            set;
        }

        public SubjectConfirmationDataType SubjectConfirmationData
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string Method
        {
            get;
            set;
        }
    }
}