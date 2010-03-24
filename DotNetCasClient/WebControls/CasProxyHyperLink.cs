using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetCasClient.Validation;

namespace DotNetCasClient.WebControls
{
    public class CasProxyHyperLink : HyperLink
    {
        private const string DEFAULT_PROXY_TICKET_URL_PARAMETER = "ticket";

        public CasProxyHyperLink()
        {
            
        }

        [Bindable(true)]
        [Category("CAS")]
        [DefaultValue(DEFAULT_PROXY_TICKET_URL_PARAMETER)]
        public string ProxyTicketUrlParameter
        {
            get
            {
                return ViewState["ProxyTicketUrlParameter"] as string ?? DEFAULT_PROXY_TICKET_URL_PARAMETER;
            }
            set
            {
                ViewState["ProxyTicketUrlParameter"] = value;
            }
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            if (Enabled && !IsEnabled)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
            }
            
            base.AddAttributesToRender(writer);
            
            string navigateUrl = NavigateUrl;
            if ((navigateUrl.Length > 0) && IsEnabled)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Href, CasAuthentication.GetProxyRedirectUrl(navigateUrl));
            }
            navigateUrl = Target;
            if (navigateUrl.Length > 0)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Target, navigateUrl);
            }
        }
    }
}
