using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;

namespace DotNetCasProxyDemoApp
{
    public partial class Form1 : Form
    {
        private string _Service;

        private NameValueCollection GetQueryStringParameters()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                if (ApplicationDeployment.CurrentDeployment.ActivationUri != null) {
                    return HttpUtility.ParseQueryString(ApplicationDeployment.CurrentDeployment.ActivationUri.Query);
                }
            }
            return null;
        }


        public Form1()
        {
            InitializeComponent();

            if (ApplicationDeployment.CurrentDeployment.ActivationUri != null)
            {
                _Service = ApplicationDeployment.CurrentDeployment.ActivationUri.AbsoluteUri;
                _Service = _Service.Substring(0, _Service.IndexOf('?'));
                ServiceField.Text = _Service;
            }

            NameValueCollection query = GetQueryStringParameters();            
            if (query == null)
            {
                MessageBox.Show("This application is only intended to be executed via WebStart with the following URL parameters: ticket, verifyUrl");
            }
            else
            {
                StringBuilder errorBuilder = new StringBuilder();
                
                if (query["ticket"] == null)
                {
                    errorBuilder.AppendLine(" - 'ticket' parameter is missing");                   
                } else
                {
                    TicketField.Text = query["ticket"];
                }
                
                if (query["verifyUrl"] == null)
                {
                    errorBuilder.AppendLine(" - 'verifyUrl' parameter is missing");
                } else
                {
                    VerifyUrlField.Text = query["verifyUrl"];
                }

                if (errorBuilder.Length != 0)
                {
                    MessageBox.Show("The following errors occurred:" + Environment.NewLine + errorBuilder);
                    return;
                }

                try
                {
                    string validateUrl = query["verifyUrl"] + "?service=" + HttpUtility.UrlEncode(_Service) + "&ticket=" + query["ticket"];

                    FinalValidationUrlField.Text = validateUrl;

                    HttpWebRequest request = (HttpWebRequest) WebRequest.Create(validateUrl);
                    using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            ServerResponseField.Text = reader.ReadToEnd();
                        }
                    }
                } 
                catch (Exception exc)
                {
                    ServerResponseField.Text = exc.ToString();
                }
            }
        }
    }
}
