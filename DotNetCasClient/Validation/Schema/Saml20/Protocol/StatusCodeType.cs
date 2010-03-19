using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Protocol
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:protocol")]
    [XmlRoot("StatusCode", Namespace="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable=false)]
    public class StatusCodeType {
        public StatusCodeType StatusCode
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string Value
        {
            get;
            set;
        }
    }
}