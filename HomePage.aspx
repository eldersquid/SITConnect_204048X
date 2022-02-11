<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HomePage.aspx.cs" Inherits="SITConnect_204048X.HomePage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <fieldset>
                <legend>Home Page</legend>
                <br />
            <br />

            <asp:Label ID="success_label" runat="server" Text=""></asp:Label>
            <asp:Label ID="email_label" runat="server" Text=""></asp:Label>
                <br />
                <br />
                <asp:Button ID="logout_button" runat="server" OnClick="logoutMethod" Text="Logout" Visible="False" />
            <br />
            <br />
            <asp:Button ID="changePW_button" runat="server" OnClick="changePW" Text="Change Password" />
                <br />
                <br />
                <asp:Label ID="error_label" runat="server" Text=""></asp:Label>
            <br />
                <br />
                <asp:Label ID="session_label" runat="server" Text=""></asp:Label>
            </fieldset>
    </form>
</body>
</html>
