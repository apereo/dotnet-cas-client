/*
 * Licensed to Apereo under one or more contributor license
 * agreements. See the NOTICE file distributed with this work
 * for additional information regarding copyright ownership.
 * Apereo licenses this file to you under the Apache License,
 * Version 2.0 (the "License"); you may not use this file
 * except in compliance with the License. You may obtain a
 * copy of the License at:
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on
 * an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

#pragma warning disable 1591

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.Saml20.Assertion.Condition;
using DotNetCasClient.Validation.Schema.Saml20.Assertion.Statement;
using DotNetCasClient.Validation.Schema.XmlDsig;

namespace DotNetCasClient.Validation.Schema.Saml20.Assertion
{
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:oasis:names:tc:SAML:2.0:assertion")]
    [XmlRoot("Assertion", Namespace="urn:oasis:names:tc:SAML:2.0:assertion", IsNullable=false)]
    public class AssertionType {
        public NameIdType Issuer
        {
            get;
            set;
        }

        [XmlElement(Namespace="http://www.w3.org/2000/09/xmldsig#")]
        public SignatureType Signature
        {
            get;
            set;
        }

        public SubjectType Subject
        {
            get;
            set;
        }

        public ConditionsType Conditions
        {
            get;
            set;
        }

        public AdviceType Advice
        {
            get;
            set;
        }

        [XmlElement("AttributeStatement", typeof(AttributeStatementType))]
        [XmlElement("AuthnStatement", typeof(AuthnStatementType))]
        [XmlElement("AuthzDecisionStatement", typeof(AuthzDecisionStatementType))]
        [XmlElement("Statement", typeof(StatementAbstractType))]
        public StatementAbstractType[] Items
        {
            get;
            set;
        }

        [XmlAttribute]
        public string Version
        {
            get;
            set;
        }

        [XmlAttribute("ID", DataType="ID")]
        public string Id
        {
            get;
            set;
        }

        [XmlAttribute]
        public DateTime IssueInstant
        {
            get;
            set;
        }
    }
}

#pragma warning restore 1591