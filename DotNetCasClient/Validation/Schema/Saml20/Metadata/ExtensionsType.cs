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
    [XmlRoot("Extensions", Namespace="urn:oasis:names:tc:SAML:2.0:metadata", IsNullable=false)]
    public class ExtensionsType {
        [XmlAnyElement]
        public XmlElement[] Any
        {
            get;
            set;
        }
    }
}