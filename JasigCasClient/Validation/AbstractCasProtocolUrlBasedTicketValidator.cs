using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace JasigCasClient.Validation
{
    /// <summary>
    /// Abstract class that knows the protocol for validating a CAS ticket
    /// </summary>
    abstract class AbstractCasProtocolUrlBasedTicketValidator : AbstractUrlBasedTicketValidator
    {

        protected AbstractCasProtocolUrlBasedTicketValidator(string casServerUrlPrefix) : base(casServerUrlPrefix) { }

        protected override string retrieveResponseFromServer(Uri validationUrl, string ticket)
        {
            // TODO do we really need this? can we get response from server inline
            // why do add "\n" to lines?
            // using(request) ???
            try
            {
                WebRequest request = WebRequest.Create(validationUrl);
                WebResponse response = request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream());

                string line;
                StringBuilder builder = new StringBuilder(255);

                lock (builder)
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        builder.AppendLine(line);
                    }
                }
                return builder.ToString();
            }
            catch (IOException e)
            {
                log.Error(e, e);
                return null;
            }
        }
    }
}
