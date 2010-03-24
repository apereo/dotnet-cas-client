using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Cas20
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="http://www.yale.edu/tp/cas")]
    public class ProxySuccess {
        internal ProxySuccess() { }

        [XmlElement("proxyTicket")]
        public string ProxyTicket {
            get;
            set;
        }
    }
}