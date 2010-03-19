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
    [XmlRoot("KeyValue", Namespace="http://www.w3.org/2000/09/xmldsig#", IsNullable=false)]
    public class KeyValueType {
        [XmlAnyElement]
        [XmlElement("DSAKeyValue", typeof(DsaKeyValueType))]
        [XmlElement("RSAKeyValue", typeof(RsaKeyValueType))]
        public object Item
        {
            get;
            set;
        }

        [XmlText]
        public string[] Text
        {
            get;
            set;
        }
    }
}