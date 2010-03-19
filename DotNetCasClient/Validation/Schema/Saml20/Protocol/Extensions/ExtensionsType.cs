using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Protocol.Extensions
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(TypeName="ExtensionsType", Namespace="urn:oasis:names:tc:SAML:2.0:protocol")]
    [XmlRoot("Extensions", Namespace="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable=false)]
    public class ExtensionsType {
        [XmlAnyElement]
        public XmlElement[] Any
        {
            get;
            set;
        }
    }
}