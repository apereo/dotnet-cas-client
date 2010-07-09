/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.XmlDsig
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="http://www.w3.org/2000/09/xmldsig#")]
    [XmlRoot("SignatureMethod", Namespace="http://www.w3.org/2000/09/xmldsig#", IsNullable=false)]
    public class SignatureMethodType {
        [XmlElement("HMACOutputLength", DataType = "integer")]
        public string HmacOutputLength
        {
            get;
            set;
        }

        [XmlText]
        [XmlAnyElement]
        public XmlNode[] Any
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string Algorithm
        {
            get;
            set;
        }
    }
}