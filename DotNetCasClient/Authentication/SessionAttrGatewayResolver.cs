using System.Web.SessionState;
using DotNetCasClient.Session;

namespace DotNetCasClient.Authentication
{
    public sealed class SessionAttrGatewayResolver : IGatewayResolver
    {
        /// <summary>
        /// The name for the session attribute created when a request is being
        /// gatewayed.
        /// </summary>
        const string CONST_CAS_GATEWAY = "_const_cas_gateway_";

        /// <summary>
        /// Determines the current gatewayed status and then sets the status to 'not gatewayed'.
        /// </summary>
        /// <param name="serviceUrl">the service url</param>
        /// <returns>the starting gatewayed status</returns>
        public bool WasGatewayed(string serviceUrl)
        {
            bool result = IsGatewayed();
            HttpSessionState session = SessionUtils.GetSession();
            if (session != null)
            {
                session.Remove(CONST_CAS_GATEWAY);
            }
            return result;
        }

        /// <summary>
        /// Determines the current gatewayed status and makes no changes.
        /// </summary>
        /// <returns>the current gatewayed status</returns>
        public bool IsGatewayed()
        {
            HttpSessionState session = SessionUtils.GetSession();
            if (session == null)
            {
                return false;
            }
            return (session[CONST_CAS_GATEWAY] != null);
        }

        /// <summary>
        /// Stores the request for gatewaying, which changes the gatewayed status to
        /// <code>true</code> and returns the service url to use for redirection,
        /// based on the submitted service URL with modifications as needed.
        /// </summary>
        /// <param name="serviceUrl">the original service url</param>
        /// <returns>the service url for redirection</returns>
        public string StoreGatewayInformation(string serviceUrl)
        {
            HttpSessionState session = SessionUtils.GetSession();
            if (session != null)
            {
                session.Add(CONST_CAS_GATEWAY, "yes");
            }
            return serviceUrl;
        }

        /// <summary>
        /// Constructs the QueryString entry needed for the Cas server login authentication
        /// redirect.
        /// </summary>
        /// <param name="prefix">
        /// data to be prepended to the constructed value, if that value is not empty.
        /// Typically this is either '?' or '&'.
        /// </param>
        /// <returns>the QueryString value, which may be the empty string</returns>
        public string RedirectQueryString(string prefix)
        {
            if (IsGatewayed())
            {
                return string.Format("{0}{1}", prefix, "gateway=\"true\"");
            }
            return "";
        }
    }
}
