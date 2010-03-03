namespace DotNetCasClient
{
    /// <summary>
    /// Lists the possible states of the gateway feature. 
    /// </summary>
    public enum GatewayStatus
    {
        /// <summary>
        /// Gateway authentication has not been attempted or the client is not 
        /// accepting session cookies
        /// </summary>
        NotAttempted,

        /// <summary>
        /// Gateway authentication is in progress
        /// </summary>
        Attempting,
            
        /// <summary>
        /// The Gateway authentication attempt was successful.  Gateway 
        /// authentication will not be attempted in subsequent requests
        /// </summary>
        Success,

        /// <summary>
        /// The Gateway authentication attempt was attempted, but a service 
        /// ticket was not returned.  Gateway authentication will not be 
        /// attempted in subsequent requests.
        /// </summary>
        Failed
    }
}