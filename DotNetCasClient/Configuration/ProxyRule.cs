/*
 * Licensed to Jasig under one or more contributor license
 * agreements. See the NOTICE file distributed with this work
 * for additional information regarding copyright ownership.
 * Jasig licenses this file to you under the Apache License,
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

using System.Configuration;
using System.Security.Permissions;
using System.Web;

namespace DotNetCasClient.Configuration
{
    //TODO: Finish this -- based on 
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public sealed class ProxyRule : ConfigurationElement
    {
        private bool _ActionModified;
        private readonly string _ElementName;
        private bool _DataReady;
        private readonly string _MachineName;

        public ProxyRuleAction Action
        {
            get;
            set;
        }

        public string ActionString
        {
            get;
            set;
        }

        public string Url
        {
            get;
            set;
        }

        internal ProxyRule(string machineName)
        {
            ActionString = ProxyRuleAction.Allow.ToString();
            _ElementName = "allow";
            this._MachineName = machineName;
        }

        public ProxyRule(ProxyRuleAction action, string machineName)
            : this(machineName)
        {
            this._MachineName = machineName;
        }

        public override bool Equals(object obj)
        {
            ProxyRule rule = obj as ProxyRule;
            bool flag = false;
            if (rule != null)
            {
                flag = (rule.Url == Url);
            }
            return flag;
        }

        public override int GetHashCode()
        {
            string str = Url.ToString();
            if (str == null)
            {
                str = string.Empty;
            }
            return str.GetHashCode();
        }

        private string ExpandName(string name)
        {
            string str = name;
            return (_MachineName + name.Substring(1));
        }

        /*
        private void EvaluateData()
        {
            if (!dataReady)
            {
                if (Users.Count > 0)
                {
                    foreach (string str in Users)
                    {
                        if (str.Length > 1)
                        {
                            int num = str.IndexOfAny(new char[] { '*', '?' });
                            if (num >= 0)
                            {
                                object[] args = new object[] { str[num].ToString(CultureInfo.InvariantCulture) };
                                throw new ConfigurationErrorsException("x");
                            }
                        }
                        if (str.Equals("*"))
                        {
                            allUsersSpecified = true;
                        }
                        if (str.Equals("?"))
                        {
                            anonUserSpecified = true;
                        }
                    }
                }
                if (Roles.Count > 0)
                {
                    foreach (string str2 in Roles)
                    {
                        if (str2.Length > 0)
                        {
                            int num2 = str2.IndexOfAny(new char[] { '*', '?' });
                            if (num2 >= 0)
                            {
                                object[] objArray2 = new object[] { str2[num2].ToString(CultureInfo.InvariantCulture) };
                                throw new ConfigurationErrorsException("x");
                            }
                        }
                    }
                }
                everyone = allUsersSpecified && (Verbs.Count == 0);
                rolesExpanded = CreateExpandedCollection(Roles);
                usersExpanded = CreateExpandedCollection(Users);
                if ((Roles.Count == 0) && (Users.Count == 0))
                {
                    throw new ConfigurationErrorsException("x");
                }
                dataReady = true;
            }
        }

        protected override void PostDeserialize()
        {
            EvaluateData();
        }

        protected override void PreSerialize(XmlWriter writer)
        {
            EvaluateData();
        }

        protected override void Reset(ConfigurationElement parentElement)
        {
            ProxyRule rule = parentElement as ProxyRule;
            if (rule != null)
            {
                rule.UpdateUsersRolesVerbs();
            }
            base.Reset(parentElement);
            EvaluateData();
        }

        protected override void ResetModified()
        {
            actionModified = false;
            base.ResetModified();
        }

        protected override bool SerializeElement(XmlWriter writer, bool serializeCollectionKey)
        {
            bool flag = false;
            UpdateUsersRolesVerbs();
            if (!base.SerializeElement(null, false))
            {
                return flag;
            }
            if (writer != null)
            {
                writer.WriteStartElement(elementName);
                flag |= base.SerializeElement(writer, false);
                writer.WriteEndElement();
                return flag;
            }
            return (flag | base.SerializeElement(writer, false));
        }

        protected override void Unmerge(ConfigurationElement sourceElement, ConfigurationElement parentElement, ConfigurationSaveMode saveMode)
        {
            ProxyRule rule = parentElement as ProxyRule;
            ProxyRule rule2 = sourceElement as ProxyRule;
            if (rule != null)
            {
                rule.UpdateUsersRolesVerbs();
            }
            if (rule2 != null)
            {
                rule2.UpdateUsersRolesVerbs();
            }
            base.Unmerge(sourceElement, parentElement, saveMode);
        }
        */

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return Properties;
            }
        }
    }
}