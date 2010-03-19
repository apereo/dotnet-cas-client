using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Assertion.Condition
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
    [XmlRoot("ProxyRestriction", Namespace="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable=false)]
    public class ProxyRestrictionType : ConditionAbstractType {
        [XmlElement("Audience", DataType="anyURI")]
        public string[] Audience
        {
            get;
            set;
        }

        [XmlAttribute(DataType="nonNegativeInteger")]
        public string Count
        {
            get;
            set;
        }
    }
}