using System;
using System.Web;
using System.Web.SessionState;
using DotNetCasClient.Configuration;
using DotNetCasClient.Utils;
using log4net;

namespace DotNetCasClient.Session
{
  /// <summary>
  /// Establishes contract for a provider of CAS single sign out functionality.
  /// </summary>
  /// <author>Catherine D. Winfrey (.Net)</author>
  interface ISingleSignOutHandler
  {
    /// <summary>
    /// Performs single sign out processing of the received HttpRequest.
    /// <remarks>
    /// Possible actions:
    /// <list type="number">
    /// <item><description>
    /// perform the single sign out processing if the request is a CAS
    /// logoutRequest
    /// </description></item>
    /// <item><description>
    /// no-op for all other types of requests
    /// </description></item>
    /// </remarks>
    /// </summary>
    /// <param name="application">the current application</param>
    /// <returns>
    /// <code>true</code> if the request is a CAS logoutRequest, which has
    /// now been processed; otherwise returns <code>false</code>, indicating
    /// that the request is <b>not</b> a CAS logoutRequest so the
    /// execution was a no-op
    /// </returns>
    bool ProcessRequest(HttpApplication application);

    /// <summary>
    /// Saves state information needed to respond to a CAS server
    /// logout request for single sign out functionality.
    /// </summary>
    /// <param name="application">the current HttpApplication</param>
    /// <param name="clientKey">the client key</param>
    /// <param name="serverKey">the server key</param>
    void StoreState(HttpApplication application,
                                    string serverKey,
                                    string clientKey);
  }
}
