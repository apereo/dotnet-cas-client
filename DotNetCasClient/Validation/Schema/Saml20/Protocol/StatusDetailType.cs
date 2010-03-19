using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Protocol
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:protocol")]
    [XmlRoot("StatusDetail", Namespace="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable=false)]
    public class StatusDetailType {
        [XmlAnyElement]
        public XmlElement[] Any
        {
            get;
            set;
        }
    }
}