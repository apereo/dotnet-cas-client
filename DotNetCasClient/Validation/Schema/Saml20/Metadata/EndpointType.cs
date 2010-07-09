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

namespace DotNetCasClient.Validation.Schema.Saml20.Metadata
{
    [XmlInclude(typeof(IndexedEndpointType))]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:metadata")]
    [XmlRoot("SingleLogoutService", Namespace="urn:oasis:names:tc:SAML:2.0:metadata", IsNullable=false)]
    public class EndpointType {
        [XmlAnyElement]
        public XmlElement[] Any
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string Binding
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string Location
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string ResponseLocation
        {
            get;
            set;
        }

        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr
        {
            get;
            set;
        }
    }
}