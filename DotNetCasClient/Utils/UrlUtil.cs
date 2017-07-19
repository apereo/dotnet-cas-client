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
using System.Text;
using System.Web;

namespace DotNetCasClient.Utils
{
    /// <summary>
    /// An internal class used to generate and modify URLs
    /// as needed for redirection and external communication.
    /// </summary>
    /// <remarks>
    /// See https://wiki.jasig.org/display/CASC/UrlUtil+Methods for additional
    /// information including sample output of each method.
    /// </remarks>
    /// <author>Scott Holodak</author>
    public sealed class UrlUtil
    {
        /// <summary>
        /// Constructs the URL to use for redirection to the CAS server for login
        /// </summary>
        /// <remarks>
        /// The server name is not parsed from the request for security reasons, which
        /// is why the service and server name configuration parameters exist.
        /// </remarks>
        /// <returns>The redirection URL to use</returns>
        public static string ConstructLoginRedirectUrl(bool gateway, bool renew)
        {
            if (gateway && renew)
            {
                throw new ArgumentException("Gateway and Renew parameters are mutually exclusive and cannot both be True");
            }

            CasAuthentication.Initialize();

            EnhancedUriBuilder ub = new EnhancedUriBuilder(CasAuthentication.FormsLoginUrl);
            ub.QueryItems.Set(CasAuthentication.TicketValidator.ServiceParameterName, HttpUtility.UrlEncode(ConstructServiceUrl(gateway)));

            if (renew)
            {
                ub.QueryItems.Add("renew", "true");
            }
            else if (gateway)
            {
                ub.QueryItems.Add("gateway", "true");
            }

            string url = ub.Uri.AbsoluteUri;

            return url;
        }

        /// <summary>
        /// Constructs a service URL using configured values in the following order:
        /// 1.  if not empty, the value configured for Service is used
        /// - otherwise -
        /// 2.  the value configured for ServerName is used together with HttpRequest
        ///     data
        /// </summary>
        /// <remarks>
        /// The server name is not parsed from the request for security reasons, which
        /// is why the service and server name configuration parameters exist, per Apereo
        /// website.
        /// </remarks>
        /// <returns>the service URL to use, not encoded</returns>
        public static string ConstructServiceUrl(bool gateway)
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
            ub.QueryItems.Remove(CasAuthentication.TicketValidator.ServiceParameterName);
            ub.QueryItems.Remove(CasAuthentication.TicketValidator.ArtifactParameterName);

            if (gateway)
            {
                ub.QueryItems.Set(CasAuthentication.GatewayParameterName, "true");
            }
            else
            {
                ub.QueryItems.Remove(CasAuthentication.GatewayParameterName);
            }
            return ub.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Constructs a URL used to check the validitiy of a service ticket, with or without a proxy 
        /// callback URL, and with or without requiring renewed credentials.
        /// </summary>
        /// <remarks>See CAS Protocol specification, section 2.5</remarks>
        /// <param name="serviceTicket">The service ticket to validate.</param>
        /// <param name="renew">
        /// Whether or not renewed credentials are required.  If True, ticket validation
        /// will fail for Single Sign On credentials.
        /// </param>
        /// <param name="gateway">
        /// whether or not to include gatewayResponse=true in the request (client specific).
        /// </param>
        /// <param name="customParameters">custom parameters to add to the validation URL</param>
        /// <returns>The service ticket validation URL to use</returns>
        public static string ConstructValidateUrl(string serviceTicket, bool gateway, bool renew, NameValueCollection customParameters)
        {
            if (gateway && renew)
            {
                throw new ArgumentException("Gateway and Renew parameters are mutually exclusive and cannot both be True");
            }

            CasAuthentication.Initialize();

            EnhancedUriBuilder ub = new EnhancedUriBuilder(EnhancedUriBuilder.Combine(CasAuthentication.CasServerUrlPrefix, CasAuthentication.TicketValidator.UrlSuffix));
            ub.QueryItems.Add(CasAuthentication.TicketValidator.ServiceParameterName, HttpUtility.UrlEncode(ConstructServiceUrl(gateway)));
            ub.QueryItems.Add(CasAuthentication.TicketValidator.ArtifactParameterName, HttpUtility.UrlEncode(serviceTicket));

            if (renew)
            {
                ub.QueryItems.Set("renew", "true");
            }

            if (customParameters != null)
            {
                for (int i = 0; i < customParameters.Count; i++)
                {
                    string key = customParameters.AllKeys[i];
                    string value = customParameters[i];

                    ub.QueryItems.Add(key, value);
                }
            }
            return ub.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Constructs a proxy callback URL containing a ProxyCallbackParameter 
        /// (proxyResponse by default).  This URL is sent to the CAS server during a proxy
        /// ticket request and is then connected to by the CAS server. If the 'CasProxyCallbackUrl' settings is specified,
        /// its value will be used to construct the proxy url. Otherwise, `ServerName` will be used.
        /// If the CAS server cannot successfully connect (generally due to SSL configuration issues), the
        /// CAS server will refuse to send a proxy ticket. 
        /// </summary>
        /// <returns>the proxy callback URL to use</returns>
        public static string ConstructProxyCallbackUrl(bool gateway)
        {
            CasAuthentication.Initialize();

            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;

            EnhancedUriBuilder ub = null;
            if (CasAuthentication.CasProxyCallbackUrl != null && CasAuthentication.CasProxyCallbackUrl.Length > 0)
            {
                ub = new EnhancedUriBuilder(CasAuthentication.CasProxyCallbackUrl);
            }
            else
            {
                ub = new EnhancedUriBuilder(CasAuthentication.ServerName);
                ub.Path = request.Url.AbsolutePath;
            }
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
            return ub.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Constructs a proxy callback URL containing a ProxyCallbackParameter 
        /// (proxyResponse by default).  This URL is sent to the CAS server during a proxy
        /// ticket request and is then connected to by the CAS server.  If the CAS server
        /// cannot successfully connect (generally due to SSL configuration issues), the
        /// CAS server will refuse to send a proxy ticket. 
        /// </summary>
        /// <remarks>
        /// This is a .NET implementation specific method used to eliminate the need for 
        /// a special HTTP Handler.  Essentially, if the client detects an incoming request
        /// with the ProxyCallbackParameter in the URL (i.e., proxyResponse), that request 
        /// is treated specially and behaves as if it were handled by an HTTP Handler.  In 
        /// other words, this behavior may or may not short circuit the request event 
        /// processing and will not allow the underlying page to execute and transmit back to
        /// the client.  If your application does coincidentally make use of the key 
        /// 'proxyResponse' as a URL parameter, you will need to configure a custom 
        /// proxyCallbackParameter value which does not conflict with the URL parameters in
        /// your application.
        /// </remarks>
        /// <returns>the proxy callback URL to use</returns>
        public static string ConstructProxyCallbackUrl()
        {
            CasAuthentication.Initialize();

            EnhancedUriBuilder ub = new EnhancedUriBuilder(ConstructProxyCallbackUrl(false));
            ub.QueryItems.Set(CasAuthentication.ProxyCallbackParameterName, "true");

            return ub.Uri.AbsoluteUri;
        }
        
        /// <summary>
        /// Constructs a proxy ticket request URL containing both a proxy granting 
        /// ticket and a URL Encoded targetServiceUrl.  The URL returned will generally only
        /// be executed by the CAS client as a part of a proxy redirection in 
        /// CasAuthentication.ProxyRedirect(...) or CasAuthentication.GetProxyTicketIdFor(...)
        /// but may also be used by applications which require low-level access to the proxy
        /// ticket request functionality.
        /// </summary>
        /// <param name="proxyGrantingTicketId">
        /// The proxy granting ticket used to authorize the request for a proxy ticket on the 
        /// CAS server
        /// </param>
        /// <param name="targetService">
        /// The target service URL to request a proxy ticket request URL for
        /// </param>
        /// <returns>The URL to use to request a proxy ticket for the targetService specified</returns>
        public static string ConstructProxyTicketRequestUrl(string proxyGrantingTicketId, string targetService)
        {
            CasAuthentication.Initialize();

            if (String.IsNullOrEmpty(proxyGrantingTicketId))
            {
                throw new ArgumentException("For proxy ticket requests, proxyGrantingTicketId cannot be null and must be specified.");
            }

            if (String.IsNullOrEmpty(targetService))
            {
                throw new ArgumentException("For proxy ticket requests, targetService cannot be null and must be specified.");
            }

            // TODO: Make "proxy" configurable.
            EnhancedUriBuilder ub = new EnhancedUriBuilder(EnhancedUriBuilder.Combine(CasAuthentication.CasServerUrlPrefix, "proxy"));
            ub.QueryItems.Add("pgt", proxyGrantingTicketId);
            ub.QueryItems.Add("targetService", HttpUtility.UrlEncode(targetService));

            return ub.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Attempts to request a proxy ticket for the targetService specified and
        /// returns a URL appropriate for redirection to the targetService containing
        /// a ticket.
        /// </summary>
        /// <param name="targetService">The target service for proxy authentication</param>
        /// <returns>The URL of the target service with a proxy ticket included</returns>
        public static string GetProxyRedirectUrl(string targetService)
        {
            return GetProxyRedirectUrl(targetService, CasAuthentication.TicketValidator.ArtifactParameterName);
        }

        /// <summary>
        /// Attempts to request a proxy ticket for the targetService specified and
        /// returns a URL appropriate for redirection to the targetService containing
        /// a ticket.
        /// </summary>
        /// <param name="targetService">The target service for proxy authentication</param>
        /// <param name="proxyTicketUrlParameter">
        /// The name of the ticket URL parameter expected by the target service (ticket by
        /// default)
        /// </param>
        /// <returns>The URL of the target service with a proxy ticket included</returns>
        public static string GetProxyRedirectUrl(string targetService, string proxyTicketUrlParameter)
        {
            CasAuthentication.Initialize();
            
            // Todo: Is ResolveUrl(...) appropriate/necessary?  If the URL starts with ~, it shouldn't require proxy authentication
            string resolvedUrl = ResolveUrl(targetService);
            string proxyTicket = CasAuthentication.GetProxyTicketIdFor(resolvedUrl);

            EnhancedUriBuilder ub = new EnhancedUriBuilder(resolvedUrl);
            ub.QueryItems[proxyTicketUrlParameter] = proxyTicket;

            return ub.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Constructs the URL to use for redirection to the CAS server for single
        /// signout.  The CAS server will invalidate the ticket granting ticket and
        /// redirect back to the current page.  The web application must then call
        /// ClearAuthCookie and revoke the ticket from the ServiceTicketManager to sign 
        /// the client out.
        /// </summary>
        /// <returns>the redirection URL to use, not encoded</returns>
        public static string ConstructSingleSignOutRedirectUrl()
        {
            CasAuthentication.Initialize();

            // TODO: Make "logout" configurable
            EnhancedUriBuilder ub = new EnhancedUriBuilder(EnhancedUriBuilder.Combine(CasAuthentication.CasServerUrlPrefix, "logout"));
            ub.QueryItems.Set(CasAuthentication.TicketValidator.ServiceParameterName, HttpUtility.UrlEncode(ConstructServiceUrl(false)));

            return ub.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Returns a copy of the URL supplied modified to remove CAS protocol-specific
        /// URL parameters.
        /// </summary>
        /// <param name="url">The URL to remove CAS artifacts from</param>
        /// <returns>The URL supplied without CAS artifacts</returns>
        public static string RemoveCasArtifactsFromUrl(string url)
        {
            CommonUtils.AssertNotNullOrEmpty(url, "url parameter can not be null or empty.");

            CasAuthentication.Initialize();

            EnhancedUriBuilder ub = new EnhancedUriBuilder(url);
            ub.QueryItems.Remove(CasAuthentication.TicketValidator.ArtifactParameterName);
            ub.QueryItems.Remove(CasAuthentication.TicketValidator.ServiceParameterName);
            ub.QueryItems.Remove(CasAuthentication.GatewayParameterName);
            ub.QueryItems.Remove(CasAuthentication.ProxyCallbackParameterName);
            
            // ++ NETC-28
            Uri uriServerName;
            if (CasAuthentication.ServerName.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) ||
                CasAuthentication.ServerName.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                uriServerName = new Uri(CasAuthentication.ServerName);
            }
            else
            {
                // .NET URIs require scheme
                uriServerName = new Uri("https://" + CasAuthentication.ServerName);
            }

            ub.Scheme = uriServerName.Scheme;
            ub.Host = uriServerName.Host;
            ub.Port = uriServerName.Port;

            return ub.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Resolves a relative ~/Url to a Url that is meaningful to the
        /// client.
        /// <remarks>
        /// Derived from: http://weblogs.asp.net/palermo4/archive/2004/06/18/getting-the-absolute-path-in-asp-net-part-2.aspx
        /// </remarks>        
        /// </summary>
        /// <author>J. Michael Palermo IV</author>
        /// <author>Scott Holodak</author>
        /// <param name="url">The Url to resolve</param>
        /// <returns>The fullly resolved Url</returns>
        internal static string ResolveUrl(string url)
        {
            CommonUtils.AssertNotNullOrEmpty(url, "url parameter can not be null or empty.");
            if (url[0] != '~') return url;

            CasAuthentication.Initialize();

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
    }
}
