using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.XmlDsig;

namespace DotNetCasClient.Validation.Schema.Saml20.Metadata
{
    [XmlInclude(typeof(AttributeAuthorityDescriptorType))]
    [XmlInclude(typeof(PdpDescriptorType))]
    [XmlInclude(typeof(AuthnAuthorityDescriptorType))]
    [XmlInclude(typeof(SsoDescriptorType))]
    [XmlInclude(typeof(SpSsoDescriptorType))]
    [XmlInclude(typeof(IdpSsoDescriptorType))]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:metadata")]
    [XmlRoot("RoleDescriptor", Namespace="urn:oasis:names:tc:SAML:2.0:metadata", IsNullable=false)]
    public abstract class RoleDescriptorType {
        [XmlElement(Namespace="http://www.w3.org/2000/09/xmldsig#")]
        public SignatureType Signature
        {
            get;
            set;
        }

        public ExtensionsType Extensions
        {
            get;
            set;
        }

        [XmlElement("KeyDescriptor")]
        public KeyDescriptorType[] KeyDescriptor
        {
            get;
            set;
        }

        public OrganizationType Organization
        {
            get;
            set;
        }

        [XmlElement("ContactPerson")]
        public ContactType[] ContactPerson
        {
            get;
            set;
        }

        [XmlAttribute("ID", DataType="ID")]
        public string Id
        {
            get;
            set;
        }

        [XmlAttribute("validUntil")]
        public DateTime ValidUntil
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool ValidUntilSpecified
        {
            get;
            set;
        }

        [XmlAttribute("cacheDuration",DataType="duration")]
        public string CacheDuration
        {
            get;
            set;
        }

        [XmlAttribute("protocolSupportEnumeration", DataType="anyURI")]
        public string[] ProtocolSupportEnumeration
        {
            get;
            set;
        }

        [XmlAttribute("errorURL", DataType="anyURI")]
        public string ErrorUrl
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