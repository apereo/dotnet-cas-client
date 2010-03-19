using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Metadata
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:metadata")]
    [XmlRoot("ContactPerson", Namespace="urn:oasis:names:tc:SAML:2.0:metadata", IsNullable=false)]
    public class ContactType {
        public ExtensionsType Extensions
        {
            get;
            set;
        }

        public string Company
        {
            get;
            set;
        }

        public string GivenName
        {
            get;
            set;
        }

        public string SurName
        {
            get;
            set;
        }

        [XmlElement("EmailAddress", DataType="anyURI")]
        public string[] EmailAddress
        {
            get;
            set;
        }

        [XmlElement("TelephoneNumber")]
        public string[] TelephoneNumber
        {
            get;
            set;
        }

        [XmlAttribute("contactType")]
        public ContactTypeType ContactTypeType
        {
            get;
            set;
        }

        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr
        {
            get;
            set;
        }
    }
}