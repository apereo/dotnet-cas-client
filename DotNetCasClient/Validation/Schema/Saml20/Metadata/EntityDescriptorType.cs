/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.XmlDsig;

namespace DotNetCasClient.Validation.Schema.Saml20.Metadata
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:metadata")]
    [XmlRoot("EntityDescriptor", Namespace="urn:oasis:names:tc:SAML:2.0:metadata", IsNullable=false)]
    public class EntityDescriptorType {
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

        [XmlElement("AffiliationDescriptor", typeof(AffiliationDescriptorType))]
        [XmlElement("AttributeAuthorityDescriptor", typeof(AttributeAuthorityDescriptorType))]
        [XmlElement("AuthnAuthorityDescriptor", typeof(AuthnAuthorityDescriptorType))]
        [XmlElement("IDPSSODescriptor", typeof(IdpSsoDescriptorType))]
        [XmlElement("PDPDescriptor", typeof(PdpDescriptorType))]
        [XmlElement("RoleDescriptor", typeof(RoleDescriptorType))]
        [XmlElement("SPSSODescriptor", typeof(SpSsoDescriptorType))]
        public object[] Items
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

        [XmlElement("AdditionalMetadataLocation")]
        public AdditionalMetadataLocationType[] AdditionalMetadataLocation
        {
            get;
            set;
        }

        [XmlAttribute("entityID", DataType="anyURI")]
        public string EntityId
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

        [XmlAttribute("cacheDuration", DataType = "duration")]
        public string CacheDuration
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

        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr
        {
            get;
            set;
        }
    }
}