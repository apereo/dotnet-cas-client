using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using DotNetCasClient.Utils;
using log4net;

namespace DotNetCasClient.Proxy
{
    /// <summary>
    /// Implementation of a ProxyRetriever that follows the CAS 2.0 specification.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For more information, please see the <a href="http://www.ja-sig.org/products/cas/overview/protocol/index.html">
    /// CAS 2.0 specification document</a>.
    /// </para>
    /// <para>
    /// In general, this class will make a call to the CAS server with some specified parameters
    /// and receive an XML response to parse.
    /// </para>
    /// </remarks>
    /// <author>Scott Battaglia</author>
    /// <author>William G. Thompson, Jr. (.Net)</author>
    class Cas20ProxyRetriever : IProxyRetriever
    {

        /// <summary>
        /// Access to the log file
        /// </summary>
        protected static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Url to CAS server used to request a Proxy Ticket.
        /// </summary>
        private string casServerUrl;

        public Cas20ProxyRetriever(string casServerUrl)
        {
            CommonUtils.AssertNotNull(casServerUrl, "casServerUrl cannot be null.");
            this.casServerUrl = casServerUrl;
        }

        public string GetProxyTicketIdFor(string proxyGrantingTicketId, Uri targetService)
        {
            // Build ProxyTicket Request Uri
            var requestUri = new StringBuilder(this.casServerUrl.EndsWith("/") ? "" : "/");
            requestUri.Append("proxy?pgt=" + proxyGrantingTicketId + "&targetService=");
            requestUri.Append(HttpUtility.UrlEncode(targetService.ToString(), new UTF8Encoding()));

            // Try to retrieve a proxy ticket from the CAS server
		    StreamReader reader = null;
            string proxyTicketResponse = null;
			try {
				reader = new StreamReader(new WebClient().OpenRead(requestUri.ToString()));
                proxyTicketResponse = reader.ReadToEnd();
			} 
            finally
			{
			    if (reader != null)
			    {
			        reader.Close();
			    }
			}

            // check for proxyFailure
            var error = XmlUtils.GetTextForElement(proxyTicketResponse, "proxyFailure");
            if (!String.IsNullOrEmpty(error))
            {
                log.Debug(error);
                return null;
            }

            // otherwise, return the proxyTicket
            return XmlUtils.GetTextForElement(proxyTicketResponse, "proxyTicket");
        }

    }
}
