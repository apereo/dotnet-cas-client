using System;
using System.Xml;
using System.Web;
using System.Net;
using System.Diagnostics;
using System.Security;
using System.IO;

namespace DotNetCASClient
{
    /// <summary>
    /// Makes a call to the cas server to verify a proxy ticket and 
    /// returns a user name if succesful or 'failed' if not.
    /// </summary>
    public class DotNetCASClientProxyValidate
    {

        private String service;
        private String casProxyValidationURL;

        /// <summary>
        /// Initialises a new instance of the CasWebServiceClient.
        /// </summary>
        /// <param name="service">Specifies the service against which to validate the ticket 
        /// 	e.g. http://atg.uwe.ac.uk/soap/mywebservice?wsdl</param>
        /// <param name="validationURL">Specifies the URL of the validator within CAS 
        /// 	e.g. https://cas.uwe.ac.uk/cas/proxyValidate</param>
        public DotNetCASClientProxyValidate(String service, String validationURL)  {
            this.service = service;
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
            String url = casProxyValidationURL + "?ticket=" + ticket + "&service=" + service;
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
        	String url = casProxyValidationURL + "?ticket=" + ticket + "&service=" + service;
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
        
        private void LogException(Exception e, String message) {
        	try {
            		EventLog.WriteEntry("CasClientC#", message + e.Message, EventLogEntryType.Error);
            		EventLog.WriteEntry("CasClientC#", e.Source, EventLogEntryType.Error);
            } catch (Exception){
            		Console.WriteLine("Failed to log error");
            }
        	
        }

    }
}

