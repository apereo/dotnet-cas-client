/*
 * Copyright 2007-2010 The JA-SIG Collaborative. All rights reserved. See license
 * distributed with this file and available online at
 * http://www.ja-sig.org/products/cas/overview/license/index.html
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