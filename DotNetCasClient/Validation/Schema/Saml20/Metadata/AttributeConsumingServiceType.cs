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
    [XmlRoot("AttributeConsumingService", Namespace="urn:oasis:names:tc:SAML:2.0:metadata", IsNullable=false)]
    public class AttributeConsumingServiceType {
        [XmlElement("ServiceName")]
        public LocalizedNameType[] ServiceName
        {
            get;
            set;
        }

        [XmlElement("ServiceDescription")]
        public LocalizedNameType[] ServiceDescription
        {
            get;
            set;
        }

        [XmlElement("RequestedAttribute")]
        public RequestedAttributeType[] RequestedAttribute
        {
            get;
            set;
        }

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