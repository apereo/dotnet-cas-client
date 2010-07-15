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

using System;
using System.Collections;
using System.Collections.Generic;

namespace DotNetCasClient.Utils
{
    /// <summary>
    /// Utility methods for the Jasig CAS Client.
    /// </summary>
    public static class CommonUtils
    {
        /// <summary>
        /// Constant representing the IIdentity AuthenticationType for 
        /// authentications via CAS.
        /// </summary>
        public const string CAS_AUTH_TYPE = "Jasig CAS";

        /// <summary>
        /// Constant representing the IIdentity AuthenticationType for 
        /// requests received from the CAS server, such as single sign
        /// out requests.
        /// </summary>
        public const string CAS_SERVER_AUTH_TYPE = "Jasig CAS-Server";

        /// <summary>
        /// Constant to use as a key for storing a redirect URI.
        /// </summary>
        public const string CAS_KEY_REDIRECT_URI = "JasigCasParamRedirectUrl";

        /// <summary>
        /// Constant to use as a key for storing the need to perform a delayed redirect
        /// using the redirect URL stored using the key CAS_KEY_REDIRECT_URL.
        /// </summary>
        public const string CAS_KEY_REDIRECT_REQUIRED = "JasigCasParamRedirectRequired";
        /// <summary>
        /// 
        /// Constant to use as a key for storing an exception to be thrown.
        /// </summary>
        public const string CAS_KEY_EXCEPTION_TO_THROW = "JasigCasParamExceptionToThrow";

        /// <summary>
        /// Constant to use as a key for storing a CAS ticket.
        /// </summary>
        public const string CAS_KEY_TICKET = "JasigCasParamTicket";

        /// <summary>
        /// The name of the cookie that stores the ASP.NET session ID.
        /// </summary>
        public const string ASP_NET_COOKIE_NAME_SESSION_ID = "ASP.NET_SessionId";

        /// <summary>
        /// The name of the method that accessed this property, typically used for
        /// logging purposes.
        /// </summary>
        public static string MethodName
        {
            get
            {
                // TODO: Check if this relies on reflection (i.e., will not work in Low/Medium trust configurations)
                return new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name;
            }
        }

        /// <summary>
        /// The name of the parent of the method that accessed this property,
        /// typically used for logging purposes.
        /// </summary>
        public static string ParentMethodName
        {
            get
            {
                // TODO: Check if this relies on reflection (i.e., will not work in Low/Medium trust configurations)
                return new System.Diagnostics.StackTrace().GetFrame(2).GetMethod().Name;
            }
        }

        /// <summary>
        /// Checks whether the object is null.
        /// </summary>
        /// <param name="entity">the object to check</param>
        /// <param name="message">the message to display if the object is null.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the object is <code>null</code>.  Includes the provided
        /// message.
        /// </exception>
        public static void AssertNotNull(object entity, string message)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(message);
            }
        }

        /// <summary>
        /// Checks whether the collection is null or empty.
        /// </summary>
        /// <param name="c">the collection to check.</param>
        /// <param name="message">
        /// the message to display if the object is null or empty.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when the collecion is <code>null</code> or empty.  Includes the provided
        /// message.
        /// </exception>
        public static void AssertNotEmpty(ICollection c, string message)
        {
            AssertNotNull(c, message);
            if (c.Count == 0)
            {
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// Asserts that the statement is <code>true</code>.
        /// </summary>
        /// <param name="cond">the boolean to check</param>
        /// <param name="message">
        /// the message to display if the boolean is not true
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when the condition is <code>false</code>.  Includes the provided
        /// message.
        /// </exception>
        public static void AssertTrue(bool cond, string message)
        {
            if (!cond)
            {
                throw new ArgumentException(message);
            }
        }


        /// <summary>
        /// Determines if the dictionary contains a non-null entry for the specified key,
        /// that the value is of the specified type, and any string values are not empty.
        /// </summary>
        /// <param name="dataDict">the dictionary to search</param>
        /// <param name="key">the key for which a value is required</param>
        /// <param name="valueType">the required type of value</param>
        /// <param name="baseMessage">base message to display if the assertion fails</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the conditions faile.  Includes the provided message followed by a
        /// colon and the part of the checks that failed.
        /// </exception>
        public static void AssertContainsNonEmpty(IDictionary<string, Object> dataDict, string key, Type valueType, string baseMessage)
        {
            AssertNotNull(dataDict, string.Format("{0}:dictionary must not be null", baseMessage));
            
            if (!dataDict.ContainsKey(key))
            {
                throw new ArgumentException(string.Format("{0}: non-empty dictionary entry with key {1} is required", baseMessage, key));
            }
            
            Object value = dataDict[key];
            AssertNotNull(value, string.Format("{0}:dictionary value for key {1} must not be null", baseMessage, key));
            AssertTrue(value.GetType().Equals(valueType), string.Format("{0}: dictionary value for key {1} must be of type {2}", baseMessage, key, valueType.Name));

            if (typeof(string).Equals(valueType))
            {
                AssertTrue(!String.IsNullOrEmpty((string)value), string.Format("{0}:dictionary value for key {1} must be non-empty string", baseMessage, key));
            }
        }
    }
}
