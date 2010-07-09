/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20
{
    [Serializable]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
    public enum DecisionType {
        [XmlEnum("Permit")]
        Permit,
        
        [XmlEnum("Deny")]
        Deny,
        
        [XmlEnum("Indeterminate")]
        Indeterminate,
    }
}