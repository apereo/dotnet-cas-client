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
    [XmlRoot("EncryptionProperties", Namespace="http://www.w3.org/2001/04/xmlenc#", IsNullable=false)]
    public class EncryptionPropertiesType {
        [XmlElement("EncryptionProperty")]
        public EncryptionPropertyType[] EncryptionProperty
        {
            get;
            set;
        }

        [XmlAttribute(DataType="ID")]
        public string Id
        {
            get;
            set;
        }
    }
}