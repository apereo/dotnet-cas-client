/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

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
    [XmlRoot("Reference", Namespace="http://www.w3.org/2000/09/xmldsig#", IsNullable=false)]
    public class ReferenceType {
        [XmlArray]
        [XmlArrayItem("Transform", IsNullable=false)]
        public TransformType[] Transforms
        {
            get;
            set;
        }

        [XmlElement]
        public DigestMethodType DigestMethod
        {
            get;
            set;
        }

        [XmlElement(DataType="base64Binary")]
        public byte[] DigestValue
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

        [XmlAttribute("URI", DataType="anyURI")]
        public string Uri
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string Type
        {
            get;
            set;
        }
    }
}