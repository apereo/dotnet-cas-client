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
using System.Text;
using System.Web;
using log4net;

namespace DotNetCasClient.Utils
{
    ///<summary>
    /// An internal class used to generate and modify URLs
    /// as needed for redirection and external communication.
    ///</summary>
    internal sealed class UrlUtil
    {
        private static readonly ILog Log = LogManager.GetLogger("UrlUtil");

        /// <summary>
        /// Resolves a relative ~/Url to a Url that is meaningful to the
        /// client.
        /// <remarks>
        /// Derived from: http://weblogs.asp.net/palermo4/archive/2004/06/18/getting-the-absolute-path-in-asp-net-part-2.aspx
        /// </remarks>        
        /// <author>J. Michael Palermo IV</author>
        /// <author>Scott Holodak</author>
        /// </summary>
        /// <param name="url">The Url to resolve</param>
        /// <returns>The fullly resolved Url</returns>
        internal static string ResolveUrl(string url)
        {
            CasAuthentication.Initialize();

            if (url == null) throw new ArgumentNullException("url", "url can not be null");
            if (url.Length == 0) throw new ArgumentException("The url can not be an empty string", "url");
            if (url[0] != '~') return url;

            string applicationPath = HttpContext.Current.Request.ApplicationPath;
            if (url.Length == 1) return applicationPath;

            // assume url looks like ~somePage 
            int indexOfUrl = 1;

            // determine the middle character 
            string midPath = ((applicationPath ?? string.Empty).Length > 1) ? "/" : string.Empty;

            // if url looks like ~/ or ~\ change the indexOfUrl to 2 
            if (url[1] == '/' || url[1] == '\\') indexOfUrl = 2;

            return applicationPath + midPath + url.Substring(indexOfUrl);
        }

        /// <summary>
        /// Constructs a service uri using configured values in the following order:
        /// 1.  if not empty, the value configured for Service is used
        /// - otherwise -
        /// 2.  the value configured for ServerName is used together with HttpRequest
        ///     data
        /// </summary>
        /// <remarks>
        /// The server name is not parsed from the request for security reasons, which
        /// is why the service and server name configuration parameters exist, per Jasig
        /// website.
        /// </remarks>
        /// <returns>the service URI to use, not encoded</returns>
        internal static string ConstructServiceUri(bool gateway)
        {
            CasAuthentication.Initialize();

            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;

            StringBuilder buffer = new StringBuilder();
            if (!(CasAuthentication.ServerName.StartsWith("https://") || CasAuthentication.ServerName.StartsWith("http://")))
            {
                buffer.Append(request.IsSecureConnection ? "https://" : "http://");
            }
            buffer.Append(CasAuthentication.ServerName);

            EnhancedUriBuilder ub = new EnhancedUriBuilder(buffer.ToString());
            ub.Path = request.Url.AbsolutePath;
            ub.QueryItems.Add(request.QueryString);
            ub.QueryItems.Remove(CasAuthentication.TicketValidator.ArtifactParameterName);

            if (gateway)
            {
                ub.QueryItems.Set(CasAuthentication.GatewayParameterName, "true");
            }
            else
            {
                ub.QueryItems.Remove(CasAuthentication.GatewayParameterName);
            }

            string url = ub.Uri.AbsoluteUri;

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("{0}:return generated serviceUri: {1}", CommonUtils.MethodName, url);
            }
            
            return url;
        }

        internal static string ConstructProxyCallbackUrl()
        {
            CasAuthentication.Initialize();

            EnhancedUriBuilder ub = new EnhancedUriBuilder(ConstructServiceUri(false));
            ub.QueryItems.Set(CasAuthentication.ProxyCallbackParameterName, "true");

            string url = ub.Uri.AbsoluteUri;

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("{0}:return generated serviceUri: {1}", CommonUtils.MethodName, url);
            }

            return url;
        }
        
        internal static string ConstructProxyTicketRequestUrl(string proxyGrantingTicketId, string targetService)
        {
            CasAuthentication.Initialize();

            EnhancedUriBuilder ub = new EnhancedUriBuilder(EnhancedUriBuilder.Combine(CasAuthentication.CasServerUrlPrefix, "proxy"));
            ub.QueryItems.Add("pgt", proxyGrantingTicketId);
            ub.QueryItems.Add("targetService", HttpUtility.UrlEncode(targetService));

            string url = ub.Uri.AbsoluteUri;

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("{0}:return generated proxy ticket request Uri: {1}", CommonUtils.MethodName, url);
            }

            return url;
        }

        internal static string GetProxyRedirectUrl(string targetServiceUrl)
        {
            return GetProxyRedirectUrl(targetServiceUrl, "ticket");
        }

        internal static string GetProxyRedirectUrl(string targetServiceUrl, string proxyTicketUrlParameter)
        {
            CasAuthentication.Initialize();

            string resolvedUrl = ResolveUrl(targetServiceUrl);
            string proxyTicket = CasAuthentication.GetProxyTicketIdFor(resolvedUrl);

            EnhancedUriBuilder ub = new EnhancedUriBuilder(resolvedUrl);
            ub.QueryItems[proxyTicketUrlParameter] = proxyTicket;

            return ub.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Constructs the URL to use for redirection to the CAS server for login
        /// </summary>
        /// <remarks>
        /// The server name is not parsed from the request for security reasons, which
        /// is why the service and server name configuration parameters exist.
        /// </remarks>
        /// <returns>the redirection URL to use</returns>
        internal static string ConstructLoginRedirectUrl(bool gateway, bool renew)
        {
            if (gateway && renew)
            {
                throw new ArgumentException("Gateway and Renew parameters are mutually exclusive and cannot both be True");
            }

            CasAuthentication.Initialize();

            EnhancedUriBuilder ub = new EnhancedUriBuilder(CasAuthentication.FormsLoginUrl);
            ub.QueryItems.Set(CasAuthentication.TicketValidator.ServiceParameterName, HttpUtility.UrlEncode(ConstructServiceUri(gateway)));

            if (renew)
            {
                ub.QueryItems.Add("renew", "true");
            }
            else if (gateway)
            {
                ub.QueryItems.Add("gateway", "true");
            }

            string url = ub.Uri.AbsoluteUri;
            
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("{0}: redirectToUrl=>{1}<", CommonUtils.MethodName, url);
            }

            return url;
        }

        internal string ConstructValidateRedirectUrl(string serviceTicket, bool requireRenewedCredentials)
        {
            CasAuthentication.Initialize();

            EnhancedUriBuilder ub = new EnhancedUriBuilder(EnhancedUriBuilder.Combine(CasAuthentication.CasServerUrlPrefix, "validate"));
            ub.QueryItems.Add(CasAuthentication.TicketValidator.ServiceParameterName, HttpUtility.UrlEncode(ConstructServiceUri(false)));
            ub.QueryItems.Add(CasAuthentication.TicketValidator.ArtifactParameterName, serviceTicket);

            if (requireRenewedCredentials)
            {
                ub.QueryItems.Set("renew", "true");
            }

            string url = ub.Uri.AbsoluteUri;
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("{0}: redirectToUrl=>{1}<", CommonUtils.MethodName, url);
            }

            return url;
        }

        internal static string ConstructServiceValidateRedirectUrl(string serviceTicket, bool requestProxyCallback, bool requireRenewedCredentials)
        {
            CasAuthentication.Initialize();

            EnhancedUriBuilder ub = new EnhancedUriBuilder(EnhancedUriBuilder.Combine(CasAuthentication.CasServerUrlPrefix, "serviceValidate"));
            ub.QueryItems.Add(CasAuthentication.TicketValidator.ServiceParameterName, HttpUtility.UrlEncode(ConstructServiceUri(false)));
            ub.QueryItems.Add(CasAuthentication.TicketValidator.ArtifactParameterName, HttpUtility.UrlEncode(serviceTicket));

            if (requestProxyCallback)
            {
                ub.QueryItems.Set("pgtUrl", HttpUtility.UrlEncode(ConstructProxyCallbackUrl()));
            }

            if (requireRenewedCredentials)
            {
                ub.QueryItems.Set("renew", "true");
            }

            string url = ub.Uri.AbsoluteUri;
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("{0}: redirectToUrl=>{1}<", CommonUtils.MethodName, url);
            }

            return url;
        }

        /// <summary>
        /// Constructs the URL to use for redirection to the CAS server for single
        /// signout.  The CAS server will invalidate the ticket granting ticket and
        /// redirect back to the current page.  The web application must then call
        /// ClearAuthCookie and revoke the ticket from the ServiceTicketManager to sign 
        /// the client out.
        /// </summary>
        /// <returns>the redirection URL to use, not encoded</returns>
        internal static string ConstructSingleSignOutRedirectUrl()
        {
            CasAuthentication.Initialize();

            EnhancedUriBuilder ub = new EnhancedUriBuilder(EnhancedUriBuilder.Combine(CasAuthentication.CasServerUrlPrefix, "logout"));
            ub.QueryItems.Set(CasAuthentication.TicketValidator.ServiceParameterName, HttpUtility.UrlEncode(ConstructServiceUri(false)));

            string url = ub.Uri.AbsoluteUri;

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("{0}: redirectToUrl=>{1}<", CommonUtils.MethodName, url);
            }

            return url;
        }

        internal static string RemoveCasArtifactsFromUrl(string url)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            CasAuthentication.Initialize();

            EnhancedUriBuilder ub = new EnhancedUriBuilder(url);
            ub.QueryItems.Remove(CasAuthentication.TicketValidator.ArtifactParameterName);
            ub.QueryItems.Remove(CasAuthentication.GatewayParameterName);
            ub.QueryItems.Remove(CasAuthentication.ProxyCallbackParameterName);

            string result = ub.Uri.AbsoluteUri;
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("{0}: redirectToUrl=>{1}<", CommonUtils.MethodName, result);
            }
            return result;
        }
    }
}
