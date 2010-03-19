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
    [XmlRoot("SignatureValue", Namespace="http://www.w3.org/2000/09/xmldsig#", IsNullable=false)]
    public class SignatureValueType {
        [XmlAttribute(DataType="ID")]
        public string Id
        {
            get;
            set;
        }

        [XmlText(DataType="base64Binary")]
        public byte[] Value
        {
            get;
            set;
        }
    }
}