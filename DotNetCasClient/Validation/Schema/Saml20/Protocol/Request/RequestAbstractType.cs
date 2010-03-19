using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.Saml20.Assertion;
using DotNetCasClient.Validation.Schema.Saml20.Protocol.SubjectQuery;
using DotNetCasClient.Validation.Schema.XmlDsig;

namespace DotNetCasClient.Validation.Schema.Saml20.Protocol.Request
{
    [XmlInclude(typeof(NameIdMappingRequestType))]
    [XmlInclude(typeof(LogoutRequestType))]
    [XmlInclude(typeof(ManageNameIdRequestType))]
    [XmlInclude(typeof(ArtifactResolveType))]
    [XmlInclude(typeof(AuthnRequestType))]
    [XmlInclude(typeof(SubjectQueryAbstractType))]
    [XmlInclude(typeof(AuthzDecisionQueryType))]
    [XmlInclude(typeof(AttributeQueryType))]
    [XmlInclude(typeof(AuthnQueryType))]
    [XmlInclude(typeof(AssertionIdRequestType))]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:protocol")]
    public abstract class RequestAbstractType {
        [XmlElement(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
        public NameIdType Issuer
        {
            get;
            set;
        }

        [XmlElement(Namespace="http://www.w3.org/2000/09/xmldsig#")]
        public SignatureType Signature
        {
            get;
            set;
        }

        public Extensions.ExtensionsType Extensions
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

        [XmlAttribute]
        public string Version
        {
            get;
            set;
        }

        [XmlAttribute]
        public DateTime IssueInstant
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string Destination
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string Consent
        {
            get;
            set;
        }
    }
}