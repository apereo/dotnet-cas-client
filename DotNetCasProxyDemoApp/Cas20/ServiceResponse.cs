using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Cas20
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.yale.edu/tp/cas")]
    [XmlRoot("serviceResponse", Namespace = "http://www.yale.edu/tp/cas", IsNullable = false)]
    public class ServiceResponse
    {
        internal ServiceResponse() { }

        public static ServiceResponse ParseResponse(string responseXml)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ServiceResponse));
            StringReader sr = new StringReader(responseXml);
            return (ServiceResponse)xs.Deserialize(sr);
        }

        [XmlElement("authenticationFailure", typeof(AuthenticationFailure))]
        [XmlElement("authenticationSuccess", typeof(AuthenticationSuccess))]
        [XmlElement("proxyFailure", typeof(ProxyFailure))]
        [XmlElement("proxySuccess", typeof(ProxySuccess))]
        public object Item
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool IsAuthenticationFailure
        {
            get
            {
                return (Item != null && Item is AuthenticationFailure);
            }
        }

        [XmlIgnore]
        public bool IsAuthenticationSuccess
        {
            get
            {
                return (Item != null && Item is AuthenticationSuccess);
            }
        }

        [XmlIgnore]
        public bool IsProxyFailure
        {
            get
            {
                return (Item != null && Item is ProxyFailure);
            }
        }

        [XmlIgnore]
        public bool IsProxySuccess
        {
            get
            {
                return (Item != null && Item is ProxySuccess);
            }
        }
    }
}