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
using System.Collections.Specialized;
using System.Web;

namespace DotNetCasClient.Utils
{
    /// <summary>
    /// Enhanced UriBuilder class with collection-based query string access.
    /// </summary>    
    /// <remarks>Adapted from http://forums.asp.net/t/693414.aspx</remarks>    
    /// <author>TlighT on ASP.NET forums</author>
    /// <author>Scott Holodak</author>
    public class EnhancedUriBuilder : UriBuilder
    {
        #region QueryItemCollection
        private class QueryItemCollection : NameValueCollection
        {
            private bool _IsDirty;

            internal bool IsDirty
            {
                get { return _IsDirty; }
                set { _IsDirty = value; }
            }

            public override void Add(string name, string value)
            {
                _IsDirty = true;
                base.Add(name, value);
            }

            public override void Remove(string name)
            {
                _IsDirty = true;
                base.Remove(name);
            }

            public override void Set(string name, string value)
            {
                _IsDirty = true;
                base.Set(name, value);
            }
        }
        #endregion

        private QueryItemCollection _QueryItems;
        private bool _QueryIsDirty;

        public EnhancedUriBuilder() { }
        public EnhancedUriBuilder(string uri) : base(uri) { _QueryIsDirty = true; }
        public EnhancedUriBuilder(Uri uri) : base(uri) { _QueryIsDirty = true; }
        public EnhancedUriBuilder(string schemeName, string hostName) : base(schemeName, hostName) { }
        public EnhancedUriBuilder(string scheme, string host, int portNumber) : base(scheme, host, portNumber) { }
        public EnhancedUriBuilder(string scheme, string host, int port, string pathValue) : base(scheme, host, port, pathValue) { }
        public EnhancedUriBuilder(string scheme, string host, int port, string path, string extraValue) : base(scheme, host, port, path, extraValue) { }

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

        public new string Query
        {
            get
            {
                SyncQuery();
                return base.Query;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value[0] == '?')
                {
                    value = value.Substring(1);
                }
                base.Query = value;
                _QueryIsDirty = true;
            }
        }

        public override string ToString()
        {
            SyncQuery();
            return base.ToString();
        }

        public new Uri Uri
        {
            get
            {
                SyncQuery();
                return base.Uri;
            }
        }

        private void SyncQueryItems()
        {
            if (_QueryIsDirty)
            {
                CreateItemsFromQuery();
                _QueryIsDirty = false;
            }
        }

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
    }
}