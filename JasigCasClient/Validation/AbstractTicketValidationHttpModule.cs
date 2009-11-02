using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JasigCasClient.Validation;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using JasigCasClient.Authentication;
using JasigCasClient.Utils;


namespace JasigCasClient.Validation
{
    /// <summary>
    /// The HttpModule that handles all the work of validating ticket requests.
    /// 
    /// This HttpModule can be configured with the following values:
    /// 
    /// redirectAfterValidation - redirect the CAS client to the same URL without the ticket.
    /// exceptionOnValidationFailure - throw an exception if the validation fails.  Othewise,
    /// continue processing
    /// useSession - store any of the useful information in a session attribute.
    /// TODO not sure if session is available to use in .Net at authenticationEvent.
    /// </summary>
    abstract class AbstractTicketValidationHttpModule : AbstractCasHttpModule
    {
        /// <summary>
        /// The TicketValidtor will we use to validate tickets.
        /// </summary>
        ITicketValidator ticketValidator;

        /// <summary>
        /// Specify whether the HttpModule should redirect the user agent after a
        /// successful validation to remove the ticket paramater from the query
        /// </summary>
        bool redirectAfterValidation;

        /// <summary>
        /// Determines whether an exception is thrown when there is a ticket validation failture.
        /// </summary>
        bool exceptionOnValidationFailure;

        bool useSession;

        /// <summary>
        /// Template method to return the appropriate validator.
        /// </summary>
        /// <param name="application">the HttpApplication that may be needed to contruct a validator</param>
        /// <returns>the ticket validtor</returns>
        protected ITicketValidator getTicketValidator(HttpApplication application)
        {
            return this.ticketValidator;
        }

        protected override void InitInternal(HttpApplication application)
        {
            this.exceptionOnValidationFailure = CasClientConfiguration.Config.ExceptionOnValidationFailure;
            log.Info("Setting " + CasClientConfiguration.EXCEPTION_ON_VALIDATION_FAILURE + " parameter: " + this.exceptionOnValidationFailure);

            this.redirectAfterValidation = CasClientConfiguration.Config.RedirectAfterValidation;
            log.Info("Setting " + CasClientConfiguration.REDIRECT_AFTER_VALIDATION + " parameter: " + this.redirectAfterValidation);
            
            // TODO do we even have access to Session in HttpModule
            //this.useSession = Convert.ToBoolean(getPropertyFromInitParams("userSession", "true"));
            //log.Info("Setting useSession parameter: " + this.useSession);

            this.ticketValidator = getTicketValidator(application);

            CommonUtils.AssertNotNull(this.ticketValidator, "ticketValidator cannot be null.");

            // All is good, so regisiter our event handlers
            application.BeginRequest += new EventHandler(application_BeginRequest);
        }

        /// <summary>
        /// Template method that gets executed if ticket validation succeeds.  Override if you want additional behavior to occur
        /// if ticket validation succeeds.  This method is called after all ValidationFilter processing required for a successful authentication
        /// occurs.
        /// </summary>
        /// <param name="request">the HttpApplication</param>
        /// <param name="assertion">the successful Assertion from the server.</param>
        protected void onSuccessfulValidation(HttpApplication application, IAssertion assertion)
        {
            // nothing to do here
        }

        /// <summary>
        /// Template method that gets executed if validation fails.  This method is called right after the exception is caught from the ticket validator
        /// but before any of the processing of the exception occurs.
        /// </summary>
        /// <param name="request">the HttpApplication</param>
        protected void onFailedValidation(HttpApplication application)
        {
            // nothing to do here.
        }

        void application_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpRequest request = application.Context.Request;

            // see if we have a ticket on the request
            string ticket = request[this.ArtifactParameterName];


            if (CommonUtils.IsNotBlank(ticket)) {
                if (log.IsDebugEnabled) {
                    log.Debug("Attempting to validate ticket: " + ticket);
                }

                try {
                    IAssertion assertion = this.ticketValidator.validate(ticket, constructServiceUrl(request));
 
                    if (log.IsDebugEnabled) {
                        log.Debug("Successfully authenticated user: " + assertion.Principal.Identity.Name);
                    }

                    // TODO Setup .Net User object and Forms Authentication Cookie

                    application.Context.Items.Add(CONST_CAS_ASSERTION, assertion);

                    // TODO not sure we have access to session at this point???
                    if (this.useSession) {
                        application.Session.Add(CONST_CAS_ASSERTION, assertion);
                    }
                
                    onSuccessfulValidation(application, assertion);

                } catch (TicketValidationException te) {
                    application.Context.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                    //403
                    log.Warn(te.Message, te);

                    onFailedValidation(application);

                    if(this.exceptionOnValidationFailure) {
                        throw new HttpException(te.Message);
                    }
                }

                if (this.redirectAfterValidation) {
                    log.Debug("Redirecting after successful ticket validation.");
                    application.Context.Response.Redirect(HttpUtility.UrlEncode(constructServiceUrl(request)));
                    return;
                }
            }
        }
    }
}

