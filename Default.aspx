<%@ Page Title="My Schedule" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MyScheduleWebsite._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="/styles/WelcomeStyles.css">

    <div>
        <h2>Welcome to our website</h2>
        <h4>Before you start, you need to Sign In or Sign Up</h4>
        <br />
        <br />

        <div>
            <table class="nav-justified" align="left" style="width: 20%">
                <tr>
                    <td class="text-left">
                        <asp:Button ID="btnSignIn" runat="server" Text="Sign in" PostBackUrl="~/Account/login.aspx" ForeColor="Blue" style="font-weight: bold; font-size: medium;"/>
                    </td>
                    <td class="text-left">
                        <asp:Button ID="btnSignUp" runat="server" Text="Sign up" PostBackUrl="~/Account/signUp.aspx" ForeColor="Blue" style="font-weight: bold; font-size: medium;"/>
                    </td>
                </tr>
            </table>
        </div>
    </div>

</asp:Content>
