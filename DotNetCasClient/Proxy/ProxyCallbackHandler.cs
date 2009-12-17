using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using DotNetCasClient.Configuration;
using DotNetCasClient.Utils;
using log4net;

namespace DotNetCasClient.Proxy
{
    class ProxyCallbackHandler
    {
        /// <summary>
        /// Access to the log file
        /// </summary>
        protected static readonly ILog log =
          LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string PARAM_PROXY_GRANTING_TICKET_IOU = "pgtIou";
        private const string PARAM_PROXY_GRANTING_TICKET = "pgtId";

        private string proxyReceptorUrl;

        public ProxyCallbackHandler(CasClientConfiguration config)
        {
            this.proxyReceptorUrl = config.ProxyReceptorUrl;
        }

        public bool ProcessRequest(HttpApplication application, IProxyGrantingTicketStorage proxyGrantingTicketStorage)
        {
            HttpRequest request = application.Request;
            // make sure we've got a proxyReceptorUrl configured and that this request is a ProxyCallback
            if (String.IsNullOrEmpty(this.proxyReceptorUrl) || !request.Path.EndsWith(this.proxyReceptorUrl))
            {
                return false;
            }

            string proxyGrantingTicketIou = request.Params[PARAM_PROXY_GRANTING_TICKET_IOU];
            string proxyGrantingTicket = request.Params[PARAM_PROXY_GRANTING_TICKET];

            if(string.IsNullOrEmpty(proxyGrantingTicket) || string.IsNullOrEmpty(proxyGrantingTicketIou))
            {
                // todo log.info that we handled the callback but didn't get the pgt
                application.Response.Write("");
                application.CompleteRequest();
                return true;
            }

            if (log.IsDebugEnabled)
            {
                log.Debug(string.Format("Recieved proxyGrantingTicketId [{0}] for proxyGrantingTicketIou [{1}]", proxyGrantingTicket, proxyGrantingTicketIou));
            }

            proxyGrantingTicketStorage.Save(proxyGrantingTicketIou, proxyGrantingTicket);
            application.Response.Write("<?xml version=\"1.0\"?>");
            application.Response.Write("<casClient:proxySuccess xmlns:casClient=\"http://www.yale.edu/tp/casClient\" />");
            application.CompleteRequest();
            return true;
        }

    }
}
