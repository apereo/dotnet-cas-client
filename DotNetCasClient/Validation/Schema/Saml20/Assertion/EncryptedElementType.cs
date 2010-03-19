using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.Xenc;

namespace DotNetCasClient.Validation.Schema.Saml20.Assertion
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
    [XmlRoot("EncryptedID", Namespace="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable=false)]
    public class EncryptedElementType {
        [XmlElement(Namespace="http://www.w3.org/2001/04/xmlenc#")]
        public EncryptedDataType EncryptedData
        {
            get;
            set;
        }

        [XmlElement("EncryptedKey", Namespace="http://www.w3.org/2001/04/xmlenc#")]
        public EncryptedKeyType[] EncryptedKey
        {
            get;
            set;
        }
    }
}