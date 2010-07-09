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

using System;
using System.Security;
using System.Security.Permissions;

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