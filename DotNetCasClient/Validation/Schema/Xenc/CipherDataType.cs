using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Xenc
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="http://www.w3.org/2001/04/xmlenc#")]
    [XmlRoot("CipherData", Namespace="http://www.w3.org/2001/04/xmlenc#", IsNullable=false)]
    public class CipherDataType {
        [XmlElement("CipherReference", typeof(CipherReferenceType))]
        [XmlElement("CipherValue", typeof(byte[]), DataType="base64Binary")]
        public object Item
        {
            get;
            set;
        }
    }
}