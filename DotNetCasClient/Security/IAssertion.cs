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

using System;
using System.Collections.Generic;

namespace DotNetCasClient.Security
{
    /// <summary>
    /// Represents a CAS response to a validation request.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the .Net port of org.jasig.cas.client.validation.Assertion
    /// </para>
    /// <para>
    /// Implementors should be Serializable
    /// </para>
    /// </remarks>
    /// <author>Scott Battaglia</author>
    /// <author>William G. Thompson, Jr. (.Net)</author>
    public interface IAssertion
    {
        /// <summary>
        /// The date from which this Assertion is valid.
        /// </summary>
        DateTime ValidFromDate
        {
            get;
        }

        /// <summary>
        /// The date this Assertion is valid until.
        /// </summary>
        DateTime ValidUntilDate
        {
            get;
        }

        /// <summary>
        /// The key/value pairs for attributes associated with this Assertion.
        /// </summary>
        Dictionary<string, IList<string>> Attributes
        {
            get;
        }

        /// <summary>
        /// The name of the Principal that this Assertion backs.
        /// </summary>
        string PrincipalName
        {
            get;
        }
    }
}
