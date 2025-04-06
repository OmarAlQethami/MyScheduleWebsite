<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="login.aspx.cs" 
    Inherits="MyScheduleWebsite.Account.login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="/styles/SignInStyles.css" />

    <div class="login-container">
        <div class="login-card">
            <h2 class="login-title">Welcome Back to<br>My Schedule</h2>

            <div class="form-group">
                <label for="txtUserName">Username</label>
                <asp:TextBox ID="txtUserName" runat="server" CssClass="form-control" autocomplete="off" />
            </div>

            <div class="form-group">
                <label for="txtPassword">Password</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" />
            </div>

            <asp:Label ID="lblOutput" runat="server" CssClass="output-label" />

            <div class="button-group">
                <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-primary" OnClick="btnLogin_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary" PostBackUrl="~/default.aspx" />
            </div>
        </div>
    </div>

</asp:Content>