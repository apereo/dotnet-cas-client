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
    [XmlRoot("SPKIData", Namespace="http://www.w3.org/2000/09/xmldsig#", IsNullable=false)]
    public class SpkiDataType {
        [XmlAnyElement]
        [XmlElement("SPKISexp", typeof(byte[]), DataType="base64Binary")]
        public object[] Items
        {
            get;
            set;
        }
    }
}