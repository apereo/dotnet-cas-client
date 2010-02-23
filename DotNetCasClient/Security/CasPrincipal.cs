using System;
using System.Security.Principal;
using DotNetCasClient.Proxy;
using DotNetCasClient.Utils;
using log4net;

namespace DotNetCasClient.Security
{
    /// <summary>
    /// Implementation of ICasPrincipal.
    /// </summary>
    /// <remarks>
    /// ICasPrincipal is the .Net port of
    ///   org.jasig.cas.client .authentication.AttributePrincipalImpl.
    /// </remarks>
    /// <author>Scott Battaglia</author>
    /// <author>William G. Thompson, Jr. (.Net)</author>
    [Serializable]
    public class CasPrincipal : ICasPrincipal
    {
        /// <summary>
        /// Access to the log file
        /// </summary>
        static readonly ILog log = LogManager.GetLogger(
          System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region ICasPrincipal Members

        public string GetProxyTicketFor(Uri service)
        {
            if (this.proxyGrantingTicket != null)
            {
                return this.proxyRetriever.GetProxyTicketIdFor(
                  this.proxyGrantingTicket, service);
            }
            log.Debug(string.Format("{0}:" +
              "No ProxyGrantingTicket was supplied --> returning null",
              CommonUtils.MethodName));
            return null;
        }

        /// <summary>
        /// The Assertion backing this Principal
        /// </summary>
        public IAssertion Assertion { get; private set; }
        
        #endregion

        # region IPrincipal Members

        // IIdentity associated with this IPrincipal
        public IIdentity Identity { get; private set;}
        
        public bool IsInRole(string role)
        {
            // TODO answer this with query to Attributes???
            throw new NotImplementedException();
        }

        #endregion

        # region Fields

        /// <summary>
        /// The CAS 2 ticket used to retrieve a proxy ticket
        /// </summary>
        string proxyGrantingTicket;

        /// <summary>
        /// The method to retrieve a proxy ticket from a CAS server.
        /// </summary>
        IProxyRetriever proxyRetriever;

        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new Principal backed by the supplied Assertion.
        /// </summary>
        /// <param name="assertion">
        /// the Assertion that backs this Principal
        /// </param>
        public CasPrincipal(IAssertion assertion) : this(assertion, null, null)
        { }

        /// <summary>
        /// Constructs a new Principal backed by the supplied Assertion, with
        /// proxying capabilities.
        /// </summary>
        /// <param name="assertion">
        /// the Assertion that backs this Principal
        /// </param>
        /// <param name="proxyGrantingTicket">
        /// the proxy granting ticket associated with this Principal.
        /// </param>
        /// <param name="proxyRetriever">
        /// the ProxyRetriever to call back to the CAS server.
        /// </param>
        public CasPrincipal(IAssertion assertion, 
                            string proxyGrantingTicket,
                            IProxyRetriever proxyRetriever)
        {
            CommonUtils.AssertNotNull(assertion, "assertion cannot be null.");

            this.Identity = new GenericIdentity(assertion.PrincipalName,
              CommonUtils.CAS_AUTH_TYPE);
            this.Assertion = assertion;
            this.proxyGrantingTicket = proxyGrantingTicket;
            this.proxyRetriever = proxyRetriever;
        }

        #endregion
    }
}
