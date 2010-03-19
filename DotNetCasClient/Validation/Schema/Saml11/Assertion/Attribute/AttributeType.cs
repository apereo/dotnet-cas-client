using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml11.Assertion.Attribute
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:assertion")]
    [XmlRoot("Attribute", Namespace="urn:oasis:names:tc:SAML:1.0:assertion", IsNullable=false)]
    public class AttributeType : AttributeDesignatorType {
        [XmlElement("AttributeValue")]
        public object[] AttributeValue
        {
            get;
            set;
        }
    }
}