/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.Saml20.Assertion;
using DotNetCasClient.Validation.Schema.Saml20.Protocol.StatusResponse;

namespace DotNetCasClient.Validation.Schema.Saml20.Protocol.Response
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(TypeName="ResponseType", Namespace="urn:oasis:names:tc:SAML:2.0:protocol")]
    [XmlRoot("Response", Namespace="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable=false)]
    public class ResponseType : StatusResponseType {
        [XmlElement("Assertion", typeof(AssertionType), Namespace="urn:oasis:names:tc:SAML:2.0:assertion"), XmlElement("EncryptedAssertion", typeof(EncryptedElementType), Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
        public object[] Items
        {
            get;
            set;
        }
    }
}