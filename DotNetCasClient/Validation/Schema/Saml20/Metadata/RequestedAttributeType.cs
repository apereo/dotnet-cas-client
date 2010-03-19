using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.Saml20.Assertion.Attribute;

namespace DotNetCasClient.Validation.Schema.Saml20.Metadata
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:metadata")]
    [XmlRoot("RequestedAttribute", Namespace="urn:oasis:names:tc:SAML:2.0:metadata", IsNullable=false)]
    public class RequestedAttributeType : AttributeType {
        [XmlAttribute("isRequired")]
        public bool IsRequired
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool IsRequiredSpecified
        {
            get;
            set;
        }
    }
}