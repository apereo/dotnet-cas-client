using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:profiles:SSO:ecp")]
    [XmlRoot("RelayState", Namespace="urn:oasis:names:tc:SAML:2.0:profiles:SSO:ecp", IsNullable=false)]
    public class RelayStateType {
        [XmlAttribute("mustUnderstand", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public bool MustUnderstand
        {
            get;
            set;
        }

        [XmlAttribute("actor",Form=System.Xml.Schema.XmlSchemaForm.Qualified, Namespace="http://schemas.xmlsoap.org/soap/envelope/", DataType="anyURI")]
        public string Actor
        {
            get;
            set;
        }

        [XmlText]
        public string Value
        {
            get;
            set;
        }
    }
}
