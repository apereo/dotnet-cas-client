using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml11.Assertion.Attribute
{
    [XmlInclude(typeof(AttributeType))]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:assertion")]
    [XmlRoot("AttributeDesignator", Namespace="urn:oasis:names:tc:SAML:1.0:assertion", IsNullable=false)]
    public class AttributeDesignatorType {
        [XmlAttribute]
        public string AttributeName
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string AttributeNamespace
        {
            get;
            set;
        }
    }
}