using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml11.Protocol
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:1.0:protocol")]
    [XmlRoot("StatusCode", Namespace="urn:oasis:names:tc:SAML:1.0:protocol", IsNullable=false)]
    public class StatusCodeType {
        [XmlElement]
        public StatusCodeType StatusCode
        {
            get;
            set;
        }

        [XmlAttribute]
        public XmlQualifiedName Value
        {
            get;
            set;
        }
    }
}