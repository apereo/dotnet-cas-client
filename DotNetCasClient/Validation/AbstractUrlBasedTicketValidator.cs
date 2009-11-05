using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web;
using JasigCasClient.Authentication;
using log4net;
using JasigCasClient.Utils;

namespace JasigCasClient.Validation
{
    abstract class AbstractUrlBasedTicketValidator : ITicketValidator
    {
        protected static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Prefix for the CAS server. Should be everything up to the URL endpoint, including the /.
        /// 
        /// e.g. http://cas.princeton.edu/
        /// </summary>
        string casServerUrlPrefix;

        /// <summary>
        /// Whether the request include a renew or not.
        /// </summary>
        public bool Renew { get; set; }

        /// <summary>
        /// Custom parameters to pass to the validation URL.
        /// </summary>
        IDictionary customParameters;

        protected AbstractUrlBasedTicketValidator(string casServerUrlPrefix)
        {
            CommonUtils.AssertNotNull(casServerUrlPrefix, "casServerUrlPrefix cannot be null.");
            this.casServerUrlPrefix = casServerUrlPrefix;
        }

        /// <summary>
        /// Template method for ticket validators that need to provide additional parameters to the validation URL.
        /// </summary>
        /// <param name="urlParamters">dictionary of parameters</param>
        protected void populateUrlAttributeDictionary(IDictionary urlParamters)
        {
            // nothing to do
        }

        /// <summary>
        /// The endpoint of the validation URL.  Should be relative (i.e. not start with a "/").
        /// i.e. validate or serviceValidate.
        /// </summary>
        protected abstract String UrlSuffix { get; }

        /// <summary>
        /// Constructs the URL to send the validation request to.
        /// </summary>
        /// <param name="ticket">the ticket to be validate.</param>
        /// <param name="serviceUrl">the service identifier</param>
        /// <returns>the fully constructed URL.</returns>
        protected string constructValidationUrl(string ticket, string serviceUrl)
        {
            IDictionary urlParameters = new Hashtable();

            log.Debug("Placing URL parameters in map.");
            urlParameters.Add("ticket", ticket);
            urlParameters.Add("service", encodeUrl(serviceUrl));

            if (this.Renew)
            {
                urlParameters.Add("renew", "true");
            }

            log.Debug("Calling template URL attribute map.");
            populateUrlAttributeDictionary(urlParameters);

            log.Debug("Loading custom parameters from configuration.");
            if (this.customParameters != null)
            {
                foreach (DictionaryEntry entry in customParameters)
                {
                    urlParameters.Add(entry.Key, entry.Value);
                }
            }

            string suffix = this.UrlSuffix;
            StringBuilder builder = new StringBuilder();

            int i = 0;
            lock (builder)
            {
                builder.Append(this.casServerUrlPrefix);
                if (!this.casServerUrlPrefix.EndsWith("/"))
                {
                    builder.Append("/");
                }
                builder.Append(suffix);

                foreach (DictionaryEntry entry in urlParameters)
                {
                    builder.Append(i++ == 0 ? "?" : "&");
                    if (!String.IsNullOrEmpty(entry.Value.ToString()))
                    {
                        builder.Append(entry.Key.ToString());
                        builder.Append("=");
                        builder.Append(entry.Value.ToString());
                    }
                    
                }
                return builder.ToString();
            } // lock(builder)
        }


        /// <summary>
        /// Encodes a URL
        /// </summary>
        /// <param name="url">URL to encode.</param>
        /// <returns>the encoded Url</returns>
        protected string encodeUrl(string url)
        {
            return String.IsNullOrEmpty(url) ? url : HttpUtility.UrlEncode(url, new UTF8Encoding());

        }

        /// <summary>
        /// Parses the response from the server into a CAS Assertion
        /// 
        /// throws TicketValidationException if an Assertion could not be created
        /// </summary>
        /// <param name="response">the response from the server, in any format.</param>
        /// <returns>the CAS assertion if one could be parsed from the response.</returns>
        protected abstract IAssertion parseResponseFromServer(string response);

        /// <summary>
        /// Contacts the CAS Server to retrieve the response for the ticket validation.
        /// </summary>
        /// <param name="validationUrl">the URL to send the validation request to.</param>
        /// <param name="ticket">the ticket to validate.</param>
        /// <returns>the response from the CAS server.</returns>
        protected abstract string retrieveResponseFromServer(Uri validationUrl, string ticket);

        public IAssertion validate(string ticket, string service)
        {
            string validationUrl = constructValidationUrl(ticket, service);
            if (log.IsDebugEnabled)
            {
                log.Debug("Constructing validation url: " + validationUrl);
            }

            try
            {
                log.Debug("Retrieving response from server.");
                string serverResponse = retrieveResponseFromServer(new Uri(validationUrl), ticket);

                if (serverResponse == null)
                {
                    throw new TicketValidationException("The CAS server returned no response.");
                }

                if (log.IsDebugEnabled)
                {
                    log.Debug("Server response: " + serverResponse);
                }

                return parseResponseFromServer(serverResponse);
            }
            catch (Exception e)
            {
                throw new TicketValidationException("Could not validate the ticket", e);
            }
        }
    }
}
