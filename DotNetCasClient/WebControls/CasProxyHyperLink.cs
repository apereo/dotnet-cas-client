/*
 * Licensed to Jasig under one or more contributor license
 * agreements. See the NOTICE file distributed with this work
 * for additional information regarding copyright ownership.
 * Jasig licenses this file to you under the Apache License,
 * Version 2.0 (the "License"); you may not use this file
 * except in compliance with the License. You may obtain a
 * copy of the License at:
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on
 * an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

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
