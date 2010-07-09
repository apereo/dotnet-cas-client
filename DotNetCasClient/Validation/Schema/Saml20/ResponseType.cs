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
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:profiles:SSO:ecp")]
    [XmlRoot("Response", Namespace="urn:oasis:names:tc:SAML:2.0:profiles:SSO:ecp", IsNullable=false)]
    public class ResponseType {
        [XmlAttribute("mustUnderstand", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public bool MustUnderstand
        {
            get;
            set;
        }

        [XmlAttribute("actor", Form=System.Xml.Schema.XmlSchemaForm.Qualified, Namespace="http://schemas.xmlsoap.org/soap/envelope/", DataType="anyURI")]
        public string Actor
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string AssertionConsumerServiceURL
        {
            get;
            set;
        }
    }
}