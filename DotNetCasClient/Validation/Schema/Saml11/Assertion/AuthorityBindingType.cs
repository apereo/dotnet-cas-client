using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml11.Assertion
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:assertion")]
    [XmlRoot("AuthorityBinding", Namespace="urn:oasis:names:tc:SAML:1.0:assertion", IsNullable=false)]
    public class AuthorityBindingType {
        [XmlAttribute]
        public XmlQualifiedName AuthorityKind
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string Location
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string Binding
        {
            get;
            set;
        }
    }
}