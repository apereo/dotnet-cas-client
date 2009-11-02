using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JasigCasClient.Utils;
using System.Web;


namespace JasigCasClient.Validation
{
    /// <summary>
    /// Implementation of AbstractTicketValidatorFilter that instanciates a Cas10TicketValidator.
    /// Deployers can provide the "casServerPrefix" and the "renew" attributes via the standard context or filter init
    /// parameters. 
    /// </summary>
    class Cas10TicketValidationHttpModule : AbstractTicketValidationHttpModule
    {
        protected new ITicketValidator getTicketValidator(HttpApplication application)
        {
            string casServerUrlPrefix = CasClientConfiguration.Config.CasServerUrlPrefix;
            Cas10TicketValidator validator = new Cas10TicketValidator(casServerUrlPrefix);
            validator.Renew = CasClientConfiguration.Config.Renew;
            return validator;
        }
    }
}
