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
using System.Diagnostics;

namespace DotNetCasClient.Logging
{
    /// <summary>
    /// Defines a logging category.
    /// </summary>
    /// <author>Marvin S. Addison</author>
    public class Category
    {
        private static Category config = new Category("DotNetCasClient.Config");
        private static Category httpModule = new Category("DotNetCasClient.HttpModule");
        private static Category protocol = new Category("DotNetCasClient.Protocol");
        private static Category security = new Category("DotNetCasClient.Security");

        private TraceSource source;

        private Category(string name)
        {
            source = new TraceSource(name);
        }

        internal TraceSource Source
        {
            get { return source; }
        }

        /// <summary>
        /// Gets the category name of the logger.
        /// </summary>
        public string Name
        {
            get { return source.Name; }
        }

        /// <summary>
        /// Gets the Config category. 
        /// </summary>
        public static Category Config
        {
            get { return config; }
        }

        /// <summary>
        /// Gets the HttpModule category.
        /// </summary>
        public static Category HttpModule
        {
            get { return httpModule; }
        }

        /// <summary>
        /// Gets the Protocol category.
        /// </summary>
        public static Category Protocol
        {
            get { return protocol; }
        }

        /// <summary>
        /// Gets the Security category.
        /// </summary>
        public static Category Security
        {
            get { return security; }
        }
    }
}