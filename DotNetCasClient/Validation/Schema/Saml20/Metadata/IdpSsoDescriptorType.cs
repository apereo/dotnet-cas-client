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
    [XmlRoot("IDPSSODescriptor", Namespace="urn:oasis:names:tc:SAML:2.0:metadata", IsNullable=false)]
    public class IdpSsoDescriptorType : SsoDescriptorType {
        [XmlElement("SingleSignOnService")]
        public EndpointType[] SingleSignOnService
        {
            get;
            set;
        }

        [XmlElement("NameIDMappingService")]
        public EndpointType[] NameIdMappingService
        {
            get;
            set;
        }

        [XmlElement("AssertionIDRequestService")]
        public EndpointType[] AssertionIdRequestService
        {
            get;
            set;
        }

        [XmlElement("AttributeProfile", DataType="anyURI")]
        public string[] AttributeProfile
        {
            get;
            set;
        }

        [XmlElement("Attribute", Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
        public AttributeType[] Attribute
        {
            get;
            set;
        }

        [XmlAttribute]
        public bool WantAuthnRequestsSigned
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool WantAuthnRequestsSignedSpecified
        {
            get;
            set;
        }
    }
}