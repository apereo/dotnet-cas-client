using System;
using System.Collections.Generic;
using System.Text;
using System.Web;


namespace DotNetCasClient.Authentication
{
  /// <summary>
  /// Contract for a gateway resolver that will maintain gateway status and answer queries
  /// about the status.
  /// </summary>
  interface IGatewayResolver
  {

    /// <summary>
    /// Determines the current gatewayed status and then sets the status to 'not gatewayed'.
    /// </summary>
    /// <param name="serviceUrl">the service url</param>
    /// <returns>the starting gatewayed status</returns>
    bool WasGatewayed(string serviceUrl);

    /// <summary>
    /// Determines the current gatewayed status and makes no changes.
    /// </summary>
    /// <returns>the current gatewayed status</returns>
    bool IsGatewayed();

    /// <summary>
    /// Stores the request for gatewaying, which changes the gatewayed status to
    /// <code>true</code> and returns the service url to use for redirection,
    /// based on the submitted service URL with modifications as needed.
    /// </summary>
    /// <param name="serviceUrl">the original service url</param>
    /// <returns>the service url for redirection</returns>
    string StoreGatewayInformation(string serviceUrl);
  }
}
