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

namespace DotNetCasClient.Utils
{
    /// <summary>
    /// Utility methods for the Apereo .NET CAS Client.
    /// </summary>
    /// <author>Marvin S. Addison</author>
    public static class CommonUtils
    {
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
        /// Checks whether the string is null or empty.
        /// </summary>
        /// <param name="entity">the string to check</param>
        /// <param name="message">the message to display if the string is null or empty.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the object is <code>null</code>.  Includes the provided
        /// message.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the object is an empty string.  Includes the provided
        /// message.
        /// </exception>
        public static void AssertNotNullOrEmpty(string entity, string message)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(message);
            } 
            else if (string.IsNullOrEmpty(entity))
            {
                throw new ArgumentException(message);
            }                
        }
    }
}
