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
    [XmlRoot("AuthnContext", Namespace="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable=false)]
    public class AuthnContextType {
        [XmlElement("AuthnContextClassRef", typeof(string), DataType="anyURI")]
        [XmlElement("AuthnContextDecl", typeof(object))]
        [XmlElement("AuthnContextDeclRef", typeof(string), DataType="anyURI")]
        [XmlChoiceIdentifier("ItemsElementName")]
        public object[] Items
        {
            get;
            set;
        }

        [XmlElement("ItemsElementName")]
        [XmlIgnore]
        public ItemsElementNames[] ItemsElementName
        {
            get;
            set;
        }

        [XmlElement("AuthenticatingAuthority", DataType="anyURI")]
        public string[] AuthenticatingAuthority
        {
            get;
            set;
        }

        [Serializable]
        [XmlType(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IncludeInSchema = false)]
        public enum ItemsElementNames
        {
            AuthnContextClassRef,
            AuthnContextDecl,
            AuthnContextDeclRef,
        }
    }
}