using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.Saml11.Assertion.Attribute;

namespace DotNetCasClient.Validation.Schema.Saml11.Assertion.SubjectStatement
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:assertion")]
    [XmlRoot("AttributeStatement", Namespace="urn:oasis:names:tc:SAML:1.0:assertion", IsNullable=false)]
    public class AttributeStatementType : SubjectStatementAbstractType {
        [XmlElement("Attribute")]
        public AttributeType[] Attribute
        {
            get;
            set;
        }
    }
}