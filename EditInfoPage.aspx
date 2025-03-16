<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditInfoPage.aspx.cs" Inherits="MyScheduleWebsite.EditInfoPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="/styles/EditInfoStyles.css" />

    <h2>Edit User Information</h2>

    <!-- STUDENT PANEL -->
    <asp:Panel ID="pnlStudent" runat="server" Visible="false">
        <h3>Student Information</h3>

        <label for="txtEnglishFirstName">English First Name:</label>
        <asp:TextBox ID="txtEnglishFirstName" runat="server" CssClass="form-control" />

        <label for="txtEnglishLastName">English Last Name:</label>
        <asp:TextBox ID="txtEnglishLastName" runat="server" CssClass="form-control" />

        <label for="txtArabicFirstName">Arabic First Name:</label>
        <asp:TextBox ID="txtArabicFirstName" runat="server" CssClass="form-control" />

        <label for="txtArabicLastName">Arabic Last Name:</label>
        <asp:TextBox ID="txtArabicLastName" runat="server" CssClass="form-control" />

        <label for="txtEmail">Email:</label>
        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" />

        <label for="ddlCurrentLevel">Current Level:</label>
        <asp:DropDownList ID="ddlCurrentLevel" runat="server" CssClass="form-control">
            <asp:ListItem Text="1" Value="1" />
            <asp:ListItem Text="2" Value="2" />
            <asp:ListItem Text="3" Value="3" />
            <asp:ListItem Text="4" Value="4" />
            <asp:ListItem Text="5" Value="5" />
            <asp:ListItem Text="6" Value="6" />
            <asp:ListItem Text="7" Value="7" />
            <asp:ListItem Text="8" Value="8" />
            <asp:ListItem Text="9" Value="9" />
            <asp:ListItem Text="10" Value="10" />
        </asp:DropDownList>

        <asp:Button ID="btnSaveStudent" runat="server" Text="Save Student Info" CssClass="btn btn-primary" OnClick="btnSaveStudent_Click" />
    </asp:Panel>

    <!-- ADMIN PANEL -->
    <asp:Panel ID="pnlAdmin" runat="server" Visible="false">
        <h3>Admin Information</h3>

        <label for="txtAdminEmail">Email:</label>
        <asp:TextBox ID="txtAdminEmail" runat="server" CssClass="form-control" />

        <label for="txtNewPassword">New Password:</label>
        <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password" />

        <label for="txtConfirmPassword">Confirm Password:</label>
        <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" />

        <asp:Button ID="btnSaveAdmin" runat="server" Text="Save Admin Info" CssClass="btn btn-primary" OnClick="btnSaveAdmin_Click" />
    </asp:Panel>

    <!-- FACULTY PANEL -->
    <asp:Panel ID="pnlFaculty" runat="server" Visible="false">
        <h3>Faculty Information</h3>

        <label for="txtFacultyEmail">Email:</label>
        <asp:TextBox ID="txtFacultyEmail" runat="server" CssClass="form-control" />

        <asp:Button ID="btnSaveFaculty" runat="server" Text="Save Faculty Info" CssClass="btn btn-primary" OnClick="btnSaveFaculty_Click" />
    </asp:Panel>

    <!-- BACK BUTTON -->
    <div style="margin-top: 20px;">
        <asp:Button ID="btnBack" runat="server" Text="Back to Home" CssClass="btn btn-secondary" OnClick="btnBack_Click" />
    </div>

    <!-- OUTPUT MESSAGE -->
    <asp:Label ID="lblOutput" runat="server" CssClass="text-success" Style="margin-top: 20px; display:block;" />

</asp:Content>
