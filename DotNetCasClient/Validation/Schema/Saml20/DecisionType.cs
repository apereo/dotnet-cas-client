using System;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20
{
    [Serializable]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
    public enum DecisionType {
        [XmlEnum("Permit")]
        Permit,
        
        [XmlEnum("Deny")]
        Deny,
        
        [XmlEnum("Indeterminate")]
        Indeterminate,
    }
}