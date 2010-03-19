using System;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml11.Assertion
{
    [Serializable]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:assertion")]
    public enum DecisionType {        
        Permit,        
        Deny,        
        Indeterminate,
    }
}