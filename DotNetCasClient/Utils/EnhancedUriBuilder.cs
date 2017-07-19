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
using System.Collections.Specialized;
using System.Web;

namespace DotNetCasClient.Utils
{
    /// <summary>
    /// Enhanced UriBuilder class with collection-based query string access.
    /// </summary>    
    /// <remarks>Adapted from http://forums.asp.net/t/693414.aspx </remarks>    
    /// <author>TlighT on ASP.NET forums</author>
    /// <author>Scott Holodak</author>
    public class EnhancedUriBuilder : UriBuilder
    {
        #region QueryItemCollection
        /// <summary>
        /// A customized NameValueCollection designed to store URL parameters.
        /// </summary>
        private class QueryItemCollection : NameValueCollection
        {
            private bool _IsDirty;

            /// <summary>
            /// Indicates whether the collection is dirty (has had values 
            /// added, removed, or changed)
            /// </summary>
            internal bool IsDirty
            {
                get { return _IsDirty; }
                set { _IsDirty = value; }
            }

            /// <summary>
            /// Adds a name/value pair to the collection
            /// </summary>
            /// <param name="name">The name of the URL parameter to add</param>
            /// <param name="value">The value of the URL parameter</param>
            public override void Add(string name, string value)
            {
                _IsDirty = true;
                base.Add(name, value);
            }

            /// <summary>
            /// Removes a name/value pair from the collection
            /// </summary>
            /// <param name="name">The name of the URL parameter to remove</param>
            public override void Remove(string name)
            {
                _IsDirty = true;
                base.Remove(name);
            }

            /// <summary>
            /// Sets a new value for the given URL parameter.
            /// </summary>
            /// <param name="name">The name of the URL parameter to set</param>
            /// <param name="value">The new value of the URL parameter</param>
            public override void Set(string name, string value)
            {
                _IsDirty = true;
                base.Set(name, value);
            }
        }
        #endregion

        #region Fields
        private QueryItemCollection _QueryItems;
        private bool _QueryIsDirty;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes an EnhancedUriBuilder with no Uri data
        /// </summary>
        public EnhancedUriBuilder() { }

        /// <summary>
        /// Initializes an EnhancedUriBuilder with the Uri supplied.
        /// </summary>
        /// <param name="uri">The string representation of a Uri to initialize with</param>
        public EnhancedUriBuilder(string uri) : base(uri) { _QueryIsDirty = true; }

        /// <summary>
        /// Initializes an EnhancedUriBuilder with the Uri supplied.
        /// </summary>
        /// <param name="uri">The Uri to initialize with</param>
        public EnhancedUriBuilder(Uri uri) : base(uri) { _QueryIsDirty = true; }

        /// <summary>
        /// Initializes an EnhancedUriBuilder with the schemeName and hostName supplied.
        /// </summary>
        /// <param name="schemeName">The schemeName to initialize with</param>
        /// <param name="hostName">The hostName to initialize with</param>
        public EnhancedUriBuilder(string schemeName, string hostName) : base(schemeName, hostName) { }
        
        /// <summary>
        /// Initializes an EnhancedUriBuilder with the scheme, host, and portNumber supplied.
        /// </summary>
        /// <param name="scheme">The scheme to initialize with</param>
        /// <param name="host">The host to initialize with</param>
        /// <param name="portNumber">The portNumber to initialize with</param>
        public EnhancedUriBuilder(string scheme, string host, int portNumber) : base(scheme, host, portNumber) { }

        /// <summary>
        /// Initializes an EnhancedUriBuilder with the scheme, host, port, and pathValue supplied.
        /// </summary>
        /// <param name="scheme">The scheme to initialize with</param>
        /// <param name="host">The host to initialize with</param>
        /// <param name="port">The port to initialize with</param>
        /// <param name="pathValue">The pathValue to initialize with</param>
        public EnhancedUriBuilder(string scheme, string host, int port, string pathValue) : base(scheme, host, port, pathValue) { }

        /// <summary>
        /// Initializes an EnhancedUriBuilder with the scheme, host, port, path, and extraValue supplied.
        /// </summary>
        /// <param name="scheme">The scheme to initialize with</param>
        /// <param name="host">The host to initialize with</param>
        /// <param name="port">The port to initialize with</param>
        /// <param name="path">The path to initialize with</param>
        /// <param name="extraValue">The extraValue to initialize with</param>
        public EnhancedUriBuilder(string scheme, string host, int port, string path, string extraValue) : base(scheme, host, port, path, extraValue) { }
        #endregion

        #region Properties
        /// <summary>
        /// The current Uri value representing the current state of the EnhancedUriBuilder.
        /// </summary>
        public new Uri Uri
        {
            get
            {
                SyncQuery();
                return base.Uri;
            }
        }

        /// <summary>
        /// The collection of query string items included in the Uri.
        /// </summary>
        public NameValueCollection QueryItems
        {
            get
            {
                if (_QueryItems == null)
                {
                    _QueryItems = new QueryItemCollection();
                }
                SyncQueryItems();
                return _QueryItems;
            }
        }

        /// <summary>
        /// The query portion of the Uri
        /// </summary>
        public new string Query
        {
            get
            {
                SyncQuery();
                return base.Query;
            }
            set
            {
                CommonUtils.AssertNotNull(value, "The Query property cannot be null.");

                if (value[0] == '?')
                {
                    value = value.Substring(1);
                }

                base.Query = value;
                _QueryIsDirty = true;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Concatenates two portions of a Url, stripping duplicate forward-slashes.
        /// </summary>
        /// <param name="hostOrPathPortion">The left-hand side of the Url to be combined</param>
        /// <param name="pathPortion">The right-hand side of the Url to be combined</param>
        /// <returns></returns>
        public static string Combine(string hostOrPathPortion, string pathPortion)
        {
            if (hostOrPathPortion == null && pathPortion == null)
            {
                return string.Empty;
            }

            if (pathPortion == null)
            {
                return hostOrPathPortion;
            }

            if (hostOrPathPortion == null)
            {
                return pathPortion;
            }

            if (!hostOrPathPortion.EndsWith("/") && !pathPortion.StartsWith("/"))
            {
                return hostOrPathPortion + "/" + pathPortion;
            }

            if (hostOrPathPortion.EndsWith("/") && pathPortion.StartsWith("/"))
            {
                return hostOrPathPortion + pathPortion.Substring(1);
            }

            return hostOrPathPortion + pathPortion;
        }

        /// <summary>
        /// The string representation of the EnhancedUriBuilder
        /// </summary>
        /// <remarks>
        /// Use of this method is discouraged.  You will generally want to return the 
        /// Uri.RawUri property instead as ToString() will include standard port numbers
        /// where they generally are not necessary (:80 and :443).
        /// </remarks>
        /// <returns></returns>
        public override string ToString()
        {
            SyncQuery();
            return base.ToString();
        }
        #endregion
        
        #region Private Methods
        /// <summary>
        /// Refreshes the query item collection if the query is marked as dirty.
        /// </summary>
        private void SyncQueryItems()
        {
            if (_QueryIsDirty)
            {
                CreateItemsFromQuery();
                _QueryIsDirty = false;
            }
        }

        /// <summary>
        /// Rebuilds the query item collection from its string representation.
        /// </summary>
        private void CreateItemsFromQuery()
        {
            _QueryItems.Clear();

            if (base.Query.Length > 0)
            {
                string query = HttpUtility.UrlDecode(base.Query.Substring(1));
                string[] items = query.Split('&');

                foreach (string item in items)
                {
                    if (item.Length > 0)
                    {
                        string[] namevalue = item.Split('=');
                        _QueryItems.Add(namevalue[0], namevalue.Length > 1 ? namevalue[1] : String.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes the string representation of the query if the query item collection is marked as dirty.
        /// </summary>
        private void SyncQuery()
        {
            if (_QueryItems != null)
            {
                // First check if queryItems has been cleared (using 
                // QueryItems.Clear()), because this doesn't 
                // update dirty flag!!!
                if (_QueryItems.Count == 0)
                {
                    base.Query = "";
                }
                else if (_QueryItems.IsDirty)
                {
                    //Console.WriteLine(">> Sync Query");
                    CreateQueryFromItems();
                }

                _QueryItems.IsDirty = false;
            }
        }

        /// <summary>
        /// Rebuilds the string representation of the query from the query item collection.
        /// </summary>
        private void CreateQueryFromItems()
        {
            string query = "";

            foreach (string key in _QueryItems.AllKeys)
            {
                string[] values = _QueryItems.GetValues(key);
                if (values != null)
                {
                    foreach (string value in values)
                    {
                        query += (key + "=" + value + "&");
                    }
                }
            }

            if (query.Length > 0)
            {
                query = query.Substring(0, query.Length - 1);
            }

            base.Query = query;
        }
        #endregion
    }
}