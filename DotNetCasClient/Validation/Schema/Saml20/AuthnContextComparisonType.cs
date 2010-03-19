using System;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20
{
    [Serializable]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:protocol")]
    public enum AuthnContextComparisonType {
        [XmlEnum("exact")]
        Exact,
        
        [XmlEnum("minimum")]
        Minimum,
        
        [XmlEnum("maximum")]
        Maximum,
        
        [XmlEnum("better")]
        Better,
    }
}