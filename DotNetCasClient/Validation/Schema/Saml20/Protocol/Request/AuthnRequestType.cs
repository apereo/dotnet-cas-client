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
using DotNetCasClient.Validation.Schema.Saml20.Assertion.Condition;

namespace DotNetCasClient.Validation.Schema.Saml20.Protocol.Request
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:protocol")]
    [XmlRoot("AuthnRequest", Namespace="urn:oasis:names:tc:SAML:2.0:protocol", IsNullable=false)]
    public class AuthnRequestType : RequestAbstractType {
        [XmlElement(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
        public SubjectType Subject
        {
            get;
            set;
        }

        [XmlElement("NameIDPolicy")]
        public NameIdPolicyType NameIdPolicy
        {
            get;
            set;
        }

        [XmlElement(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
        public ConditionsType Conditions
        {
            get;
            set;
        }

        public RequestedAuthnContextType RequestedAuthnContext
        {
            get;
            set;
        }

        public ScopingType Scoping
        {
            get;
            set;
        }

        [XmlAttribute]
        public bool ForceAuthn
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool ForceAuthnSpecified
        {
            get;
            set;
        }

        [XmlAttribute]
        public bool IsPassive
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool IsPassiveSpecified
        {
            get;
            set;
        }

        [XmlAttribute(DataType="anyURI")]
        public string ProtocolBinding
        {
            get;
            set;
        }

        [XmlAttribute]
        public ushort AssertionConsumerServiceIndex
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool AssertionConsumerServiceIndexSpecified
        {
            get;
            set;
        }

        [XmlAttribute("AssertionConsumerServiceURL", DataType = "anyURI")]
        public string AssertionConsumerServiceUrl
        {
            get;
            set;
        }

        [XmlAttribute]
        public ushort AttributeConsumingServiceIndex
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool AttributeConsumingServiceIndexSpecified
        {
            get;
            set;
        }

        [XmlAttribute]
        public string ProviderName
        {
            get;
            set;
        }
    }
}