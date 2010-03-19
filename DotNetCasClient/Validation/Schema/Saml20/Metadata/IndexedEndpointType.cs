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
    [XmlRoot("ArtifactResolutionService", Namespace="urn:oasis:names:tc:SAML:2.0:metadata", IsNullable=false)]
    public class IndexedEndpointType : EndpointType {
        [XmlAttribute("index")]
        public ushort Index
        {
            get;
            set;
        }

        [XmlAttribute("isDefault")]
        public bool IsDefault
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool IsDefaultSpecified
        {
            get;
            set;
        }
    }
}