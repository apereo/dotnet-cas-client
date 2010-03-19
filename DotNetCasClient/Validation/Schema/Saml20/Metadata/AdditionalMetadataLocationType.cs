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
    [XmlRoot("AdditionalMetadataLocation", Namespace="urn:oasis:names:tc:SAML:2.0:metadata", IsNullable=false)]
    public class AdditionalMetadataLocationType {
        [XmlAttribute("namespace", DataType="anyURI")]
        public string Namespace
        {
            get;
            set;
        }

        [XmlText(DataType="anyURI")]
        public string Value
        {
            get;
            set;
        }
    }
}