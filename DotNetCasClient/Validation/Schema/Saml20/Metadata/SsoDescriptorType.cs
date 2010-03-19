using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Metadata
{
    [XmlInclude(typeof(SpSsoDescriptorType))]
    [XmlInclude(typeof(IdpSsoDescriptorType))]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:metadata")]
    public abstract class SsoDescriptorType : RoleDescriptorType {
        [XmlElement("ArtifactResolutionService")]
        public IndexedEndpointType[] ArtifactResolutionService
        {
            get;
            set;
        }

        [XmlElement("SingleLogoutService")]
        public EndpointType[] SingleLogoutService
        {
            get;
            set;
        }

        [XmlElement("ManageNameIDService")]
        public EndpointType[] ManageNameIdService
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