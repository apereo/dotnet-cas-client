using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml11.Assertion
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:assertion")]
    [XmlRoot("SubjectLocality", Namespace="urn:oasis:names:tc:SAML:1.0:assertion", IsNullable=false)]
    public class SubjectLocalityType {
        [XmlAttribute("IPAddress")]
        public string IpAddress
        {
            get;
            set;
        }

        [XmlAttribute("DNSAddress")]
        public string DnsAddress
        {
            get;
            set;
        }
    }
}