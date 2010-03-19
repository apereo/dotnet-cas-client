using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.XmlDsig;

namespace DotNetCasClient.Validation.Schema.Saml11.Assertion
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:assertion")]
    [XmlRoot("SubjectConfirmation", Namespace="urn:oasis:names:tc:SAML:1.0:assertion", IsNullable=false)]
    public class SubjectConfirmationType {
        [XmlElement("ConfirmationMethod", DataType="anyURI")]
        public string[] ConfirmationMethod
        {
            get;
            set;
        }

        [XmlElement]
        public object SubjectConfirmationData
        {
            get;
            set;
        }

        [XmlElement(Namespace="http://www.w3.org/2000/09/xmldsig#")]
        public KeyInfoType KeyInfo
        {
            get;
            set;
        }
    }
}