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
    [XmlRoot("RSAKeyValue", Namespace="http://www.w3.org/2000/09/xmldsig#", IsNullable=false)]
    public class RsaKeyValueType {
        [XmlElement(DataType="base64Binary")]
        public byte[] Modulus
        {
            get;
            set;
        }

        [XmlElement(DataType="base64Binary")]
        public byte[] Exponent
        {
            get;
            set;
        }
    }
}