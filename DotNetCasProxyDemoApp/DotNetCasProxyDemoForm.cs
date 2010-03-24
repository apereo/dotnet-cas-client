using System;
using System.Collections.Specialized;
using System.Deployment.Application;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;
using System.Xml.Serialization;
using DotNetCasClient.Validation.Schema.Cas20;

namespace DotNetCasProxyDemoApp
{
    public partial class DotNetCasProxyDemoForm : Form
    {
        private readonly string _Service;

        private static NameValueCollection GetQueryStringParameters()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                if (ApplicationDeployment.CurrentDeployment.ActivationUri != null) {
                    return HttpUtility.ParseQueryString(ApplicationDeployment.CurrentDeployment.ActivationUri.Query);
                }
            }
            return null;
        }

        public DotNetCasProxyDemoForm()
        {
            InitializeComponent();

            if (ApplicationDeployment.CurrentDeployment.ActivationUri != null)
            {
                _Service = ApplicationDeployment.CurrentDeployment.ActivationUri.AbsoluteUri;
                _Service = _Service.Substring(0, _Service.IndexOf('?'));
                TargetServiceNameField.Text = _Service;

                NameValueCollection query = GetQueryStringParameters();
                if (query == null)
                {
                    MessageBox.Show("This application is only intended to be executed via WebStart with the following URL parameters: proxyTicket, verifyUrl");
                }
                else
                {
                    StringBuilder errorBuilder = new StringBuilder();

                    if (query["proxyTicket"] == null)
                    {
                        errorBuilder.AppendLine(" - 'proxyTicket' parameter is missing");
                    }
                    else
                    {
                        ProxyTicketField.Text = query["proxyTicket"];
                    }

                    if (query["verifyUrl"] == null)
                    {
                        errorBuilder.AppendLine(" - 'verifyUrl' parameter is missing");
                    }
                    else
                    {
                        ProxyValidateUrl.Text = query["verifyUrl"];
                    }

                    if (errorBuilder.Length != 0)
                    {
                        MessageBox.Show("The following errors occurred:" + Environment.NewLine + errorBuilder);
                        return;
                    }


                    Uri validateUrl = new Uri(query["verifyUrl"] + "?service=" + HttpUtility.UrlEncode(_Service) + "&ticket=" + query["proxyTicket"]);
                    ValidationUrlField.Text = validateUrl.AbsoluteUri;

                    ValidateProxyTicket(validateUrl);
                }
            }
            else
            {
                MessageBox.Show(
                    "This application was not launched via the web.  You will generally want to either forbid the application " +
                    "from running or implement RESTful CAS authentication via a login box"
                );
            }
        }

        private void ValidateProxyTicket(Uri validateUrl)
        {
            try
            {
                string xml;
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(validateUrl);
                using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        xml = reader.ReadToEnd();
                        ServerResponseField.Text = xml.Replace("\n\n\n", "\n").Replace("\n\n", "\n").Replace("\n", Environment.NewLine).Replace("\t", "  ");

                        StringReader sr = new StringReader(xml);
                            
                        XmlSerializer serializer = new XmlSerializer(typeof (ServiceResponse));
                        ServiceResponse serviceResponse = serializer.Deserialize(sr) as ServiceResponse;

                        if (serviceResponse != null)
                        {
                            if (serviceResponse.IsAuthenticationSuccess)
                            {
                                AuthenticationSuccess successResponse = (AuthenticationSuccess)serviceResponse.Item;
                                StatusField.Text = "SUCCESS";
                                StatusField.ForeColor = Color.Green;
                                StatusColorField.BackColor = Color.Green;
                                UsernameField.Text = successResponse.User;
                                ProxiesField.DataSource = successResponse.Proxies;
                            }
                            else if (serviceResponse.IsAuthenticationFailure)
                            {
                                AuthenticationFailure failureResponse = (AuthenticationFailure)serviceResponse.Item;
                                StatusField.Text = "FAILURE";
                                StatusField.ForeColor = Color.Green;
                                StatusColorField.BackColor = Color.Red;
                                UsernameLabel.Text = "Code " + failureResponse.Code;
                                MessageLabel.Text = failureResponse.Message;
                            }
                            else
                            {
                                StatusField.Text = "UNEXPECTED RESPONSE";
                                StatusColorField.BackColor = Color.Yellow;
                                StatusExceptionField.Visible = true;
                            }
                        }
                    }
                }
            } 
            catch (Exception exc)
            {
                StatusField.Text = "EXCEPTION";
                ServerResponseField.Text = exc.ToString();
                StatusColorField.BackColor = Color.Yellow;
                StatusExceptionField.Visible = true;
            }
        }
    }
}
