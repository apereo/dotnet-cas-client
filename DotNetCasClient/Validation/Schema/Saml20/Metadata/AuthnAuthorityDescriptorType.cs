using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Metadata
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:metadata")]
    [XmlRoot("AuthnAuthorityDescriptor", Namespace="urn:oasis:names:tc:SAML:2.0:metadata", IsNullable=false)]
    public class AuthnAuthorityDescriptorType : RoleDescriptorType {
        [XmlElement("AuthnQueryService")]
        public EndpointType[] AuthnQueryService
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

        [XmlElement("NameIDFormat", DataType="anyURI")]
        public string[] NameIdFormat
        {
            get;
            set;
        }
    }
}