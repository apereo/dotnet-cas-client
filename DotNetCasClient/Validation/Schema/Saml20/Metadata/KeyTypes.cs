using System;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Metadata
{
    [Serializable]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:metadata")]
    public enum KeyTypes {
        [XmlEnum("encryption")]        
        Encryption,
        
        [XmlEnum("signing")]
        Signing,
    }
}