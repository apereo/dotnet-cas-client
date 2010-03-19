using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Protocol.StatusResponse
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:protocol")]
    [XmlRoot("ArtifactResponse", Namespace="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable=false)]
    public class ArtifactResponseType : StatusResponseType {
        [XmlAnyElement]
        public XmlElement Any
        {
            get;
            set;
        }
    }
}