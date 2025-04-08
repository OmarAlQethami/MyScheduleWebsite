<%@ Page Title="My Schedule" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MyScheduleWebsite._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="/styles/WelcomeStyles.css">

    <div class="welcome-container">
        <div class="welcome-card">
            <p class="welcome-subtitle">Welcome to</p>
            <h1 class="welcome-title">My Schedule</h1>
            <p class="welcome-subtitle">Plan your ideal schedule, control your academic journey</p>
            
            <div class="welcome-content">
                <p class="welcome-text">To get started, please sign in to your account or create a new one.</p>
                
                <div class="button-container">
                    <asp:Button ID="btnSignIn" runat="server" Text="Sign In" 
                        PostBackUrl="~/Account/login.aspx" CssClass="btn btn-primary" />
                    <asp:Button ID="btnSignUp" runat="server" Text="Sign Up" 
                        PostBackUrl="~/Account/signUp.aspx" CssClass="btn btn-secondary" />
                </div>
            </div>
        </div>
    </div>

</asp:Content> 
