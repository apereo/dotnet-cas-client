/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
 */

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DotNetCasClient.Validation.Schema.Saml20.Assertion.Condition
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
    [XmlRoot("Conditions", Namespace="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable=false)]
    public class ConditionsType {
        [XmlElement("AudienceRestriction", typeof(AudienceRestrictionType)), XmlElement("Condition", typeof(ConditionAbstractType)), XmlElement("OneTimeUse", typeof(OneTimeUseType)), XmlElement("ProxyRestriction", typeof(ProxyRestrictionType))]
        public ConditionAbstractType[] Items
        {
            get;
            set;
        }

        [XmlAttribute]
        public DateTime NotBefore
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool NotBeforeSpecified
        {
            get;
            set;
        }

        [XmlAttribute]
        public DateTime NotOnOrAfter
        {
            get;
            set;
        }

        [XmlIgnore]
        public bool NotOnOrAfterSpecified
        {
            get;
            set;
        }
    }
}