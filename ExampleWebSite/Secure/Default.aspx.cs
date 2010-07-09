using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Secure_Default : System.Web.UI.Page
{
    protected void Button_Click(object sender, EventArgs e)
    {
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // RoleGridView.DataSource = Roles.GetRolesForUser();
    }
}