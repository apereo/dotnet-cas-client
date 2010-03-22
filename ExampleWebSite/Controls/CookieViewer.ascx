<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CookieViewer.ascx.cs" Inherits="Controls_CookieViewer" %>

<table class="block" cellpadding="0" cellspacing="0">
    <tr>
        <th colspan="2">
            Forms Authentication Cookie
        </th>
    </tr>
    <tr>
        <td width="25%">
            Domain
        </td>
        <td>
            <asp:Label ID="CookieDomain" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Expires
        </td>
        <td>
            <asp:Label ID="CookieExpires" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Name
        </td>
        <td>
            <asp:Label ID="CookieName" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Path
        </td>
        <td>
            <asp:Label ID="CookiePath" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Secure
        </td>
        <td>
            <asp:Label ID="CookieSecure" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Value
        </td>
        <td>
            <asp:Label ID="CookieValue" runat="server" />
        </td>
    </tr>
</table>
<br />
<table class="block" cellpadding="0" cellspacing="0">
    <tr>
        <th colspan="2">
            Forms Authentication Ticket
        </th>
    </tr>
    <tr>
        <td width="25%">
             Cookie Path
        </td>
        <td>
            <asp:Label ID="TicketCookiePath" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
             Expiration
        </td>
        <td>
            <asp:Label ID="TicketExpiration" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
             Expired
        </td>
        <td>
            <asp:Label ID="TicketExpired" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
             Is Persistent
        </td>
        <td>
            <asp:Label ID="TicketIsPersistent" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
             Issue Date
        </td>
        <td>
            <asp:Label ID="TicketIssueDate" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
             Name
        </td>
        <td>
            <asp:Label ID="TicketName" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
             User Data
        </td>
        <td>
            <asp:Label ID="TicketUserData" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
             Version
        </td>
        <td>
            <asp:Label ID="TicketVersion" runat="server" />
        </td>
    </tr>
</table>
<br />
<table class="block" cellpadding="0" cellspacing="0">
    <tr>
        <th colspan="2">
            CAS Authentication Ticket
        </th>
    </tr>
    <tr>
        <td width="25%">
            NetId
        </td>
        <td>
            <asp:Label ID="CasNetId" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Service Ticket
        </td>
        <td>
            <asp:Label ID="CasServiceTicket" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Originating Service Name
        </td>
        <td>
            <asp:Label ID="CasOriginatingServiceName" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Client Host Address
        </td>
        <td>
            <asp:Label ID="CasClientHostAddress" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Valid From Date
        </td>
        <td>
            <asp:Label ID="CasValidFromDate" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Valid Until Date
        </td>
        <td>
            <asp:Label ID="CasValidUntilDate" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Proxy Granting Ticket IOU
        </td>
        <td>
            <asp:Label ID="ProxyGrantingTicketIou" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Proxy Granting Ticket
        </td>
        <td>
            <asp:Label ID="ProxyGrantingTicket" runat="server" />
        </td>
    </tr>
    <tr>
        <td valign="top"> 
            Proxies
        </td>
        <td>
            <asp:Label ID="Proxies" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Assertion
        </td>
        <td>
            <asp:Label ID="CasAssertion" runat="server" />
        </td>
    </tr>
</table>
<br />
<table class="block" cellpadding="0" cellspacing="0">
    <tr>
        <th colspan="2">
            CAS Assertion
        </th>
    </tr>
    <tr>
        <td width="25%">
            Principal Name
        </td>
        <td>
            <asp:Label ID="AssertionPrincipalName" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Valid From Date
        </td>
        <td>
            <asp:Label ID="AssertionValidFromDate" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Valid Until Date
        </td>
        <td>
            <asp:Label ID="AssertionValidUntilDate" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            Attributes
        </td>
        <td>
            <asp:Table ID="AssertionAttributesTable" runat="server" CssClass="block" />
        </td>
    </tr>
</table>
