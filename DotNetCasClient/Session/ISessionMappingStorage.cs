using System;
using System.Web.SessionState;

namespace JasigCasClient.Session
{
  /// <summary>
  /// Contract for storage of Http sessions keyed by a server-supplied value and
  /// maintenance of a mapping between a client-supplied value and this server-supplied
  /// value.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This is the .Net port of org.jasig.cas.client.session.SessionMappingStorage
  /// </para>
  /// </remarks>
  /// <author>Scott Battaglia</author>
  /// <author>Catherine D. Winfrey (.Net)</author>
  interface ISessionMappingStorage
  {
    /// <summary>
    /// Removes an Http session from the storage, identifying the session by its
    /// server key.
    /// </summary>
    /// <param name="serverKey">
    /// server key of the session to be removed from storage
    /// </param>
    /// <returns>
    /// the session that was removed, or <code>null</code> if no session was found
    /// </returns>
    HttpSessionState RemoveSessionByServerKey(string serverKey);


    /// <summary>
    /// Removes an Http session from the storage, identifying the session by its
    /// client key.
    /// </summary>
    /// <param name="clientKey">
    /// client key of the session to be removed from storage
    /// </param>
    void RemoveSessionByClientKey(string clientKey);


    /// <summary>
    /// Adds a session to the storage using the supplied identifiers.
    /// </summary>
    /// <param name="serverKey">
    /// server key
    /// </param>
    /// <param name="clientKey">
    /// client key of the session to be removed from storage
    /// </param>
    /// <param name="session">Http session object</param>
    void AddSession(string serverKey, string clientKey, HttpSessionState session);
  }
}
