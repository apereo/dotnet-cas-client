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
using System.Web.Configuration;

[AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
public sealed class AuthorizationSection : ConfigurationSection
{
    // Fields
    private bool _EveryoneAllowed;
    private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
    private static readonly ConfigurationProperty _propRules = new ConfigurationProperty(null, typeof(AuthorizationRuleCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);

    // Methods
    static AuthorizationSection()
    {
        _properties.Add(_propRules);
    }

    protected override void PostDeserialize()
    {
        if (Rules.Count > 0)
        {
            // _EveryoneAllowed = (Rules[0].Action == AuthorizationRuleAction.Allow) && Rules[0].Everyone;
        }
    }

    // Properties
    internal bool EveryoneAllowed
    {
        get
        {
            return _EveryoneAllowed;
        }
    }

    protected override ConfigurationPropertyCollection Properties
    {
        get
        {
            return _properties;
        }
    }

    [ConfigurationProperty("", IsDefaultCollection = true)]
    public AuthorizationRuleCollection Rules
    {
        get
        {
            return (AuthorizationRuleCollection)base[_propRules];
        }
    }
}

