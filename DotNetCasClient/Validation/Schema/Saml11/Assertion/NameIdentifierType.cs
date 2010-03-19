using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml11.Assertion
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:assertion")]
    [XmlRoot("NameIdentifier", Namespace="urn:oasis:names:tc:SAML:1.0:assertion", IsNullable=false)]
    public class NameIdentifierType {
        [XmlAttribute]
        public string NameQualifier
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string Format
        {
            get;
            set;
        }

        [XmlText]
        public string Value
        {
            get;
            set;
        }
    }
}