using System;
using System.Text;
using System.Web;
using System.Web.SessionState;
using DotNetCasClient.Configuration;
using DotNetCasClient.Utils;
using log4net;

namespace DotNetCasClient.Session
{
  /// <summary>
  /// Abstract class for CAS single sign out functionality.
  /// </summary>
  /// <author>Catherine D. Winfrey (.Net)</author>
  abstract class AbstractSingleSignOutHandler : ISingleSignOutHandler
  {
    /// <summary>
    /// Access to the log file
    /// </summary>
    protected static readonly ILog log =
      LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private const string XML_SESSION_INDEX_ELEMENT_NAME = "samlp:SessionIndex";

    #region Fields
    /// <summary>
    /// Storage for Http sessions whose web pages have been processed by the CAS server.
    /// </summary>
    protected static ISessionMappingStorage SESSION_MAPPING_STORAGE = null;
    #endregion

    #region Constructor

    /// <summary>
    /// Constructs an ISingleSignOutHandler, initializing it with the supplied
    /// configuration data.
    /// </summary>
    /// <param name="config">
    /// ConfigurationManager to be used to obtain the settings needed by this
    /// single sign out handler
    /// </param>
    protected AbstractSingleSignOutHandler(CasClientConfiguration config)
    {
      if (SESSION_MAPPING_STORAGE == null) {
        SESSION_MAPPING_STORAGE = new DictionarySessionMappingStorage();
      }     
    }
    #endregion

    /// <summary>
    /// Performs single sign out processing of the received HttpRequest.
    /// <remarks>
    /// See interface class for details.
    /// </remarks>
    public virtual bool ProcessRequest(HttpApplication application)
    {
      HttpRequest request = application.Request;
      bool logoutRequestReceived = false;
      if ("POST".Equals(request.RequestType)) {
        string logoutRequest = request.Params["logoutRequest"];
        if (log.IsDebugEnabled) {
          log.Debug(string.Format("{0}:POST logoutRequest={1}",
            CommonUtils.MethodName,
            (logoutRequest == null ? "null" : logoutRequest)));
        }
        if (CommonUtils.IsNotBlank(logoutRequest)) {
          logoutRequestReceived = true;
          string serverKey = 
            XmlUtils.GetTextForElement(logoutRequest, XML_SESSION_INDEX_ELEMENT_NAME);
          if (log.IsDebugEnabled) {
            log.Debug(string.Format("{0}:serverKey=[{1}]",
              CommonUtils.MethodName, serverKey));
          }
          if (CommonUtils.IsNotBlank(serverKey)) {
            HttpSessionState session = 
              SESSION_MAPPING_STORAGE.RemoveSessionByServerKey(serverKey);
            if (session != null) {
              string sessionId = session.SessionID;
              if (log.IsDebugEnabled) {
                log.Debug(string.Format(
                  "{0}:Abandoning session[{1}] for serverKey [{2}]]",
                  CommonUtils.MethodName, sessionId, serverKey));
              }
              try {
                session.Clear();
                session.Abandon();
              } catch (Exception ex) {
                if (log.IsDebugEnabled) {
                  log.Debug(string.Format(
                    "{0}:FAILURE during abandon session [{1}] for serverKey [{2}]:{3} " +
                    CommonUtils.MethodName, sessionId, serverKey, ex.Message));
                }
              }
            }
          }
        }
      }
      return logoutRequestReceived;
    }


    /// <summary>
    /// Saves state information needed to respond to a CAS server
    /// logout request for single sign out functionality.
    /// </summary>
    /// <remarks>
    /// See interface class for details.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the current environment blocks processing, such as no access to the
    /// Http session.
    /// </exception>      
    public void StoreState(HttpApplication application,
                           string serverKey,
                           string clientKey)
    {
      if (CommonUtils.IsNotBlank(serverKey)  && CommonUtils.IsNotBlank(clientKey)) {
        HttpSessionState session = application.Session;
        if (session != null) {
          if (log.IsDebugEnabled) {
            log.Debug(string.Format("{0}:Storing single sign out information: " +
              "session[{1}] and serverKey [{2}] and clientKey[{3}]",
                CommonUtils.MethodName, session.SessionID, serverKey, clientKey));
          }
          SESSION_MAPPING_STORAGE.AddSession(serverKey, clientKey, session);
        } else {
          throw new InvalidOperationException(
            "Single sign out functionality requires access to the session");
        }
      }
    }
    /// <summary>
    /// Removes the state information stored for use with a CAS
    /// server logout request.
    /// <remarks>
    /// One use case is for an expired session.  The session itself
    /// does not need to be abandoned, but the storage needs to be
    /// updated.  This may be a common occurrence, if the browser
    /// is closed without doing a CAS logout, for example.
    /// </remarks>
    /// </summary>
    /// <param name="clientKey">the client key</param>
    public static void RemoveState(string clientKey)
    {
      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:process clientKey={1}",
          CommonUtils.MethodName,
          clientKey == null ? "null" : clientKey));
      }
      if (SESSION_MAPPING_STORAGE != null) {
        SESSION_MAPPING_STORAGE.RemoveSessionByClientKey(clientKey);
      }
    }
  }
}
