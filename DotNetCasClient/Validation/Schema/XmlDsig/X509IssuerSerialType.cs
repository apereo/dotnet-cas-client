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
    public class X509IssuerSerialType {
        [XmlElement]
        public string X509IssuerName
        {
            get;
            set;
        }

        [XmlElement(DataType="integer")]
        public string X509SerialNumber
        {
            get;
            set;
        }
    }
}