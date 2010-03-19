using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.Xenc;
using DotNetCasClient.Validation.Schema.XmlDsig;

namespace DotNetCasClient.Validation.Schema.Saml20.Metadata
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:metadata")]
    [XmlRoot("KeyDescriptor", Namespace="urn:oasis:names:tc:SAML:2.0:metadata", IsNullable=false)]
    public class KeyDescriptorType {
        [XmlElement(Namespace="http://www.w3.org/2000/09/xmldsig#")]
        public KeyInfoType KeyInfo
        {
            get;
            set;
        }

        [XmlElement("EncryptionMethod")]
        public EncryptionMethodType[] EncryptionMethod
        {
            get;
            set;
        }

        [XmlAttribute("use")]
        public KeyTypes Use
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool UseSpecified
        {
            get;
            set;
        }
    }
}