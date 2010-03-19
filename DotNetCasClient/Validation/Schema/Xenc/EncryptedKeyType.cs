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
    [XmlRoot("EncryptedKey", Namespace="http://www.w3.org/2001/04/xmlenc#", IsNullable=false)]
    public class EncryptedKeyType : EncryptedType {
        public ReferenceList ReferenceList
        {
            get;
            set;
        }

        public string CarriedKeyName
        {
            get;
            set;
        }

        [XmlAttribute]
        public string Recipient
        {
            get;
            set;
        }
    }
}