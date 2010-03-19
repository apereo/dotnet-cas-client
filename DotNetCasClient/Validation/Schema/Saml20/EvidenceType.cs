using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.Saml20.Assertion;

namespace DotNetCasClient.Validation.Schema.Saml20
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
    [XmlRoot("Evidence", Namespace="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable=false)]
    public class EvidenceType {
        [XmlElement("Assertion", typeof(AssertionType)), XmlElement("AssertionIDRef", typeof(string), DataType="NCName"), XmlElement("AssertionURIRef", typeof(string), DataType="anyURI"), XmlElement("EncryptedAssertion", typeof(EncryptedElementType)), XmlChoiceIdentifier("ItemsElementName")]
        public object[] Items
        {
            get;
            set;
        }

        [XmlElement("ItemsElementName"), XmlIgnore]
        public ItemsElementNames[] ItemsElementName
        {
            get;
            set;
        }

        [Serializable]
        [XmlType(Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", IncludeInSchema = false)]
        public enum ItemsElementNames
        {
            [XmlEnum("Assertion")]
            Assertion,

            [XmlEnum("AssertionIDRef")]
            AssertionIdRef,

            [XmlEnum("AssertionURIRef")]
            AssertionUriRef,

            [XmlEnum("EncryptedAssertion")]
            EncryptedAssertion,
        }
    }
}