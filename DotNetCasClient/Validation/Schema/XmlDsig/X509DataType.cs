using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.XmlDsig
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="http://www.w3.org/2000/09/xmldsig#")]
    [XmlRoot("X509Data", Namespace="http://www.w3.org/2000/09/xmldsig#", IsNullable=false)]
    public class X509DataType {
        [XmlAnyElement]
        [XmlElement("X509CRL", typeof(byte[]), DataType="base64Binary")]
        [XmlElement("X509Certificate", typeof(byte[]), DataType="base64Binary")]
        [XmlElement("X509IssuerSerial", typeof(X509IssuerSerialType))]
        [XmlElement("X509SKI", typeof(byte[]), DataType="base64Binary")]
        [XmlElement("X509SubjectName", typeof(string))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public object[] Items
        {
            get;
            set;
        }

        [XmlElement("ItemsElementName")]
        [XmlIgnore]
        public ItemsElementNames[] ItemsElementName
        {
            get;
            set;
        }

        [Serializable]
        [XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#", IncludeInSchema = false)]
        public enum ItemsElementNames
        {
            [XmlEnum("##any:")]
            Item,

            [XmlEnum("X509CRL")]
            X509Crl,

            [XmlEnum("X509Certificate")]
            X509Certificate,

            [XmlEnum("X509IssuerSerial")]
            X509IssuerSerial,

            [XmlEnum("X509SKI")]
            X509Ski,

            [XmlEnum("X509SubjectName")]
            X509SubjectName,
        }
    }
}