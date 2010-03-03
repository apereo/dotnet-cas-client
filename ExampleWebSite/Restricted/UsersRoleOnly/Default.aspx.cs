using System;
using System.Web.Security;
using System.Web.UI.WebControls;

public partial class Restricted_AuthenticatedUsersOnly_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            string[] allRoles = Roles.GetAllRoles();
            string[] roles = Roles.GetRolesForUser();

            RoleList.DataSource = allRoles;
            RoleList.DataBind();
            RoleList.Enabled = false;

            foreach (string role in roles)
            {
                foreach (ListItem item in RoleList.Items)
                {
                    item.Selected = (item.Value == role);
                }
            }
        }
    }
}