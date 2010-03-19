using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Protocol.Request
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:protocol")]
    [XmlRoot("ArtifactResolve", Namespace="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable=false)]
    public class ArtifactResolveType : RequestAbstractType {
        public string Artifact
        {
            get;
            set;
        }
    }
}