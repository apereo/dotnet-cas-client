using System;
using System.Xml;
using System.Web;
using System.Net;
using System.Diagnostics;
using System.Security;
using System.IO;
using System.Web.Configuration;

namespace DotNetCASClient {
    
    public abstract class LogCASExceptions {
        protected static void LogException(Exception e, String message) {
        	try {
            		EventLog.WriteEntry("CasClientC#", message + e.Message, EventLogEntryType.Error);
            		EventLog.WriteEntry("CasClientC#", e.Source, EventLogEntryType.Error);
            } catch (Exception){
            		Console.WriteLine("Failed to log error");
            }
        	
        }
    }
    
    /// <summary>
    /// Makes a call to the cas server to verify a proxy ticket and 
    /// returns a user name if succesful or 'failed' if not.
    /// </summary>
    public class DotNetCASClientProxyValidate : LogCASExceptions{

        private String serviceURL;
        private String casProxyValidationURL;

        /// <summary>
        /// Initialises a new instance of the CasWebServiceClient.
        /// </summary>
        /// <param name="service">Specifies the service against which to validate the ticket 
        /// 	e.g. http://atg.uwe.ac.uk/soap/mywebservice?wsdl</param>
        /// <param name="validationURL">Specifies the URL of the validator within CAS 
        /// 	e.g. https://cas.uwe.ac.uk/cas/proxyValidate</param>
        public DotNetCASClientProxyValidate(String service, String validationURL)  {
            this.serviceURL = service;
            this.casProxyValidationURL = validationURL;
            try {
	            if (!EventLog.SourceExists("CasClientC#")){
	            	EventLog.CreateEventSource("CasClientC#","Application");
	            }
            } catch (SecurityException e) {
            	Console.WriteLine("Could not search all the Logs for the specified source (CasClientC#): "+ e.Message);
            } catch (ArgumentException e) {
            	Console.WriteLine("Invalid Log Name: " + e.Message);
            } catch (InvalidOperationException e) {
            	Console.WriteLine("Could not open registry key: " + e.Message);
            }
        }

        /// <summary>
        /// Authenticates the users based on the ticket passed in as an argument. Passes the ticket to CAS for 
        /// validation and returns either the resulting username or 'failed' if the process of either communicating with
        /// CAS or authenticating fails.
        /// </summary>
        /// <param name="ticket">The proxy or service ticket uses to authorise with CAS</param>
        /// <returns>Either the username passed from CAS or 'failed'</returns>
        public String Authenticate(String ticket) {
            String url = casProxyValidationURL + "?ticket=" + ticket + "&service=" + serviceURL;
            String result = null;
            try {
            	XmlTextReader reader = new XmlTextReader(url);
                while (reader.Read()) {
                    if (reader.LocalName == "user") {
                        result = reader.ReadString();
                    }
                }
            reader.Close();
            } catch (FileNotFoundException e) {
            	LogException(e, "Failed to locate XML: ");
            	return "failed";
            } catch (DirectoryNotFoundException e) {
            	LogException(e, "Failed to locate XML: ");
            	return "failed";
            } catch (WebException e) {
            	LogException(e, "Problem accessing the resource - could be a network problem: ");
            	return "failed";
            } catch (UriFormatException e) {
            	LogException(e, "The URL is not properly formed: ");
            	return "failed";
            } catch (XmlException e) {
            	LogException(e, "The XML is malformed: ");
            	return "failed";
            } catch (InvalidOperationException e) {
            	LogException(e, "Invalid Operation: ");
            	return "failed";
            }
            
            if (result == null) {
            	try{
            		EventLog.WriteEntry("CasClientC#", "Failed to authenticate.", EventLogEntryType.Warning );
            	} catch (Exception){
            		Console.WriteLine("Failed to log error");
            	}
                result = "failed";
            }
            return result;
        }
        
        /// <summary>
        /// Performs the ticket validation but simply returns all the XML from CAS without trying to retrieve a 
        /// username.
        /// </summary>
        /// <param name="ticket">The proxy or service ticket uses to authorise with CAS</param>
        /// <returns>The XML from CAS - or null if an exception occurs.</returns>
        public XmlDocument GetCasXML(String ticket){
        	String url = casProxyValidationURL + "?ticket=" + ticket + "&service=" + serviceURL;
            XmlDocument xml = null;
            try {
            	xml = new XmlDocument();
            	xml.Load(new XmlTextReader(url));
            }
            catch (Exception e) {
            	LogException(e, "Failed to read and/or parse XML: ");
               return null;
            }
            return xml;
        }

    }
    
    /// <summary>
    /// Looks for a ticket in the request and uses it to authenticate the user
    /// or, if there isn't one, redirects to the CAS server based on web-config 
    /// settings.
    /// </summary>
    public class DotNetCASClientServiceValidate : LogCASExceptions{
        
        private String casLoginURL;
        private String casValidateURL;
        private String serviceURL;
        
        /// <summary>
        /// Initialises a new instance of DotNetCASClientServiceValidate
        /// </summary>
        public DotNetCASClientServiceValidate() {
            try {
	            if (!EventLog.SourceExists("CasClientC#")){
	            	EventLog.CreateEventSource("CasClientC#","Application");
	            }
            } catch (SecurityException e) {
            	Console.WriteLine("Could not search all the Logs for the specified source (CasClientC#): "+ e.Message);
            } catch (ArgumentException e) {
            	Console.WriteLine("Invalid Log Name: " + e.Message);
            } catch (InvalidOperationException e) {
            	Console.WriteLine("Could not open registry key: " + e.Message);
            }
            casLoginURL = WebConfigurationManager.AppSettings["casLoginURL"];
            casValidateURL = WebConfigurationManager.AppSettings["casValidateURL"];
            serviceURL = WebConfigurationManager.AppSettings["serviceURL"];
        }
        
       /// <summary>
       /// Performs simple authentication by either validating a ticket or redirecting
       /// the user to the CAS server in order to obtain a ticket.
       /// </summary>
       /// <param name="request">The HttpRequest which may contain the ticket</param>
       /// <param name="response">The HttpResponse in case the user needs redrecting</param>
       /// <param name="needXML">Whether or not to return a username or the XML response from CAS</param>
       /// <returns></returns>
        public String Authenticate(HttpRequest request, HttpResponse response, bool needXML){
            String ticket = request.Params["ticket"];
            String result = "";
            if (ticket == null) {
                try {
                    response.Redirect(casLoginURL + "?service=" + serviceURL);
                } catch (HttpException e){
                    LogException(e, "Failed to redirect the user to the CAS server");
                }
            } else {
                DotNetCASClientProxyValidate client = new DotNetCASClientProxyValidate(serviceURL, casValidateURL);
                if (!needXML) {
                result = client.Authenticate(ticket);
                    if (result.Equals("failed")) {
                        try {
                            EventLog.WriteEntry("CasClientC#", "Failed to authenticate based based on the given ticket", EventLogEntryType.Warning);
                        } catch (Exception){
                		    Console.WriteLine("Failed to log error");
                        }
                    }
                } else {
                    result = client.GetCasXML(ticket).InnerXml;
                }
                return result;
            }
           //This code should never be reachable
           return "";
        }
    }
}

