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

namespace DotNetCasClient.Validation.Schema.Saml20.Protocol.SubjectQuery
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:protocol")]
    [XmlRoot("AuthzDecisionQuery", Namespace="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable=false)]
    public class AuthzDecisionQueryType : SubjectQueryAbstractType {
        [XmlElement("Action", Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
        public ActionType[] Action
        {
            get;
            set;
        }

        [XmlElement(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
        public EvidenceType Evidence
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string Resource
        {
            get;
            set;
        }
    }
}