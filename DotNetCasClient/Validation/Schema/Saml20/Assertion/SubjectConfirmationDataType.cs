using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Assertion
{
    [XmlInclude(typeof(KeyInfoConfirmationDataType))]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
    [XmlRoot("SubjectConfirmationData", Namespace="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable=false)]
    public class SubjectConfirmationDataType {
        [XmlText]
        public string[] Text
        {
            get;
            set;
        }
    }
}