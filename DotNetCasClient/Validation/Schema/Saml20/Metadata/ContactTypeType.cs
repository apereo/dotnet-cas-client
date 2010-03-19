using System;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Metadata
{
    [Serializable]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:metadata")]
    public enum ContactTypeType {
        [XmlEnum("technical")]        
        Technical,

        [XmlEnum("support")]
        Support,

        [XmlEnum("administrative")]
        Administrative,

        [XmlEnum("billing")]
        Billing,

        [XmlEnum("other")]
        Other,
    }
}