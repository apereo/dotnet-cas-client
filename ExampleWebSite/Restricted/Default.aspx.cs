using System;
using System.Security;
using System.Security.Permissions;
using System.Web.Security;
using System.Web.UI.WebControls;

public partial class Restricted_AuthenticatedUsersOnly_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            TestUserIsAuthenticatedMethodResult.Text = TestUserIsAuthenticatedMethod().ToString();
        }
        catch (SecurityException)
        {
            TestUserIsAuthenticatedMethodResult.Text = "SecurityException --> False";
        }

        try
        {
            TestUsersRoleMethodResult.Text = TestUsersRoleMethod().ToString();
        }
        catch (SecurityException)
        {
            TestUsersRoleMethodResult.Text = "SecurityException --> False";
        }

        try
        {
            TestAdministratorsRoleMethodResult.Text = TestAdministratorsRoleMethod().ToString();
        }
        catch (SecurityException)
        {
            TestAdministratorsRoleMethodResult.Text = "SecurityException --> False";
        }

        try
        {
            TestUsersOrAdministratorsRoleMethodResult.Text = TestUsersOrAdministratorsRoleMethod().ToString();
        }
        catch (SecurityException)
        {
            TestUsersOrAdministratorsRoleMethodResult.Text = "SecurityException --> False";
        }

        try
        {
            TestUserIsBobMethodResult.Text = TestUserIsBobMethod().ToString();
        }
        catch (SecurityException)
        {
            TestUserIsBobMethodResult.Text = "SecurityException --> False";
        }
    }

    [PrincipalPermission(SecurityAction.Demand, Authenticated=true)]
    private static bool TestUserIsAuthenticatedMethod()
    {
        return true;
    }

    [PrincipalPermission(SecurityAction.Demand, Role = "Users")]
    private static bool TestUsersRoleMethod()
    {
        return true;
    }

    [PrincipalPermission(SecurityAction.Demand, Role = "Administrators")]
    private static bool TestAdministratorsRoleMethod()
    {
        return true;
    }

    [PrincipalPermission(SecurityAction.Demand, Name = "Bob")]
    private static bool TestUserIsBobMethod()
    {
        return true;
    }

    private static bool TestUsersOrAdministratorsRoleMethod()
    {
        return TestUsersRoleMethod() || TestAdministratorsRoleMethod();
    }
}