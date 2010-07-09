/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:protocol")]
    [XmlRoot("NameIDPolicy", Namespace="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable=false)]
    public class NameIdPolicyType {
        [XmlAttribute(DataType="anyURI")]
        public string Format
        {
            get;
            set;
        }

        [XmlAttribute("SPNameQualifier")]
        public string SpNameQualifier
        {
            get;
            set;
        }

        [XmlAttribute]
        public bool AllowCreate
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool AllowCreateSpecified
        {
            get;
            set;
        }
    }
}