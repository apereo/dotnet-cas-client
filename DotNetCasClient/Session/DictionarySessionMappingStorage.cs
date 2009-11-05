using System;
using System.Collections.Generic;
using System.Web.SessionState;
using DotNetCasClient.Utils;
using log4net;

namespace DotNetCasClient.Session
{
  /// <summary>
  /// Stores Http sessions using a CAS-server supplied key and maintains a mapping
  /// between a CAS-client supplied key and this CAS-server key using Dictionary class. 
  /// </summary>
  /// <remarks>
  /// <para>
  /// This is the .Net port of org.jasig.cas.client.session.HashmapBackedSessionMappingStorage
  /// </para>
  /// </remarks>
  /// <author>Scott Battaglia</author>
  /// <author>Catherine D. Winfrey (.Net)</author>
  sealed class DictionarySessionMappingStorage : ISessionMappingStorage
  {
    /// <summary>
    /// Access to the log file
    /// </summary>
    static readonly ILog log =
      LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// Maps the CAS-client key to the CAS-server key (typically the ticket).
    /// </summary>
    private IDictionary<string, string> mappingClientKeyToServerKey =
      new Dictionary<string, string>();

    /// <summary>
    /// Stores HttpSessionState objects keyed with the CAS-server key.
    /// </summary>
    private IDictionary<string, Object> managedSessions =
      new Dictionary<string, Object>();

    /// <summary>
    /// Maps the CAS-server key to the CAS-client key.
    /// </summary>
    private IDictionary<string, string> mappingServerKeyToClientKey =
      new Dictionary<string, string>();

    /// <summary>
    /// Removes an Http session from the storage, identifying the session by its
    /// CAS-server key.
    /// </summary>
    /// <param name="serverKey">
    /// CAS-server key of the session to be removed from storage
    /// </param>
    /// <returns>
    /// the session that was removed, or <code>null</code> if no session was found
    /// or the session removal could not be performed
    /// </returns>
    public HttpSessionState RemoveSessionByServerKey(string serverKey)
    {
      HttpSessionState session = null;
      string clientKey = null;
      if (serverKey != null) {
        lock(this.managedSessions) {
          this.LogCurrentState("start(RemoveSessionByServerKey)");
          if (this.mappingServerKeyToClientKey.ContainsKey(serverKey)) {
            clientKey = this.mappingServerKeyToClientKey[serverKey];
            this.mappingServerKeyToClientKey.Remove(serverKey);
            this.mappingClientKeyToServerKey.Remove(clientKey);
          } else {
            if (log.IsInfoEnabled) {
              log.Info(string.Format("{0}: missing serverKey mapping for {1}",
                CommonUtils.MethodName, serverKey));
            }
          }
          if (this.managedSessions.ContainsKey(serverKey)) {
            session = (HttpSessionState)this.managedSessions[serverKey];
            this.managedSessions.Remove(serverKey);
          }
          this.LogCurrentState("end(RemoveSessionByServerKey)");
        }
      } else {
        if (log.IsWarnEnabled) {
          log.Warn(string.Format("{0}: processing blocked by null serverKey",
            CommonUtils.MethodName));
        }      }
      return session;
    }

    /// <summary>
    /// Removes an Http session from the storage, identifying the session by its
    /// CAS-client key.
    /// </summary>
    /// <param name="clientKey">
    /// CAS-client key of the session to be removed from storage
    /// </param>
    public void RemoveSessionByClientKey(string clientKey)
    {
      if (clientKey != null) {
        lock (this.managedSessions) {
          this.LogCurrentState("start(RemoveSessionByClientKey)");
          if (this.mappingClientKeyToServerKey.ContainsKey(clientKey)) {
            string serverKey = this.mappingClientKeyToServerKey[clientKey];
            this.managedSessions.Remove(serverKey);
            this.mappingServerKeyToClientKey.Remove(serverKey);
            this.mappingClientKeyToServerKey.Remove(clientKey);
            this.LogCurrentState("end(RemoveSessionByClientKey");
          } else {
            if (log.IsWarnEnabled) {
              log.Warn(string.Format("{0}: processing blocked by missing mapping for clientKey {1}",
                CommonUtils.MethodName, clientKey));
            }
          }
        }
      } else {
        if (log.IsWarnEnabled) {
          log.Warn(string.Format("{0}: processing blocked by null clientKey",
            CommonUtils.MethodName));
        }
      }
    }

    /// <summary>
    /// Adds a session to the storage using the supplied identifiers.
    /// </summary>
    /// <param name="serverKey">
    /// CAS-server key
    /// </param>
    /// <param name="clientKey">
    /// CAS-client key of the session to be removed from storage
    /// </param>
    /// <param name="session">Http session object</param>
    public void AddSession(string serverKey, string clientKey, HttpSessionState session)
    {
      if (serverKey != null && clientKey != null & session != null) {
        lock (this.managedSessions) {
          string serverKeyPrev = null;
          if (this.mappingClientKeyToServerKey.ContainsKey(clientKey)) {
            serverKeyPrev = this.mappingClientKeyToServerKey[clientKey];
            if (this.managedSessions.ContainsKey(serverKeyPrev)) {
              this.managedSessions.Remove(serverKeyPrev);
            }
            if (this.mappingServerKeyToClientKey.ContainsKey(serverKeyPrev)) {
              this.mappingServerKeyToClientKey.Remove(serverKeyPrev);
            }
          }
          this.mappingClientKeyToServerKey[clientKey] = serverKey;
          this.mappingServerKeyToClientKey[serverKey] = clientKey;
          this.managedSessions[serverKey] = session;
          this.LogCurrentState("end(AddSession)");
        }
      }
    }


    /// <summary>
    /// Finds the CAS-client key associated with the a CAS-server key.
    /// </summary>
    /// <param name="serverKey">CAS-server key</param>
    /// <returns>the associated CAS-client key</returns>
    public string GetClientKey(string serverKey)
    {
      string clientKey = null;
      if (serverKey != null) {
        try {
          clientKey = this.mappingServerKeyToClientKey[serverKey];
        } catch (Exception) {
          clientKey = null;
        }
      }
      return clientKey;
    }

    /// <summary>
    /// Logs the session storage information, generally used for debugging purposes.
    /// </summary>
    /// <param name="msgPrefix">identifying label for the log entries</param>
    public void LogCurrentState(string msgPrefix)
    {
      if (log.IsDebugEnabled) {
        log.Debug(string.Format("{0}:{1}:mappingClientKeyToServerKey:{2}",
          CommonUtils.MethodName, msgPrefix, DebugUtils.DictionaryToString(
            this.mappingClientKeyToServerKey)));
        log.Debug(string.Format("{0}:{1}:mappingServerKeyToClientKey:{2}",
          CommonUtils.MethodName, msgPrefix, DebugUtils.DictionaryToString(
            this.mappingServerKeyToClientKey)));
        log.Debug(string.Format("{0}:{1}:managedSessions:{2}",
          CommonUtils.MethodName, msgPrefix, DebugUtils.DictionaryToString(this.managedSessions)));
        foreach(KeyValuePair<string, Object>kvp in this.managedSessions) {
          HttpSessionState nextSession = (HttpSessionState)kvp.Value;
          log.Debug(string.Format("{0}:{1}:nextSession={2}",
            CommonUtils.MethodName, msgPrefix, nextSession.SessionID));
        }
      }
    }
  }
}
