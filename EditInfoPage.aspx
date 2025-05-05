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
            <asp:ListItem Text="Choose your current level" Value="" Selected="True"></asp:ListItem>
        </asp:DropDownList>

        <label for="txtCurrentPasswordStudent">Current Password:</label>
        <asp:TextBox ID="txtCurrentPasswordStudent" runat="server" CssClass="form-control" TextMode="Password" />

        <label for="txtNewPasswordStudent">New Password:</label>
        <asp:TextBox ID="txtNewPasswordStudent" runat="server" CssClass="form-control" TextMode="Password" />

        <label for="txtConfirmPasswordStudent">Confirm New Password:</label>
        <asp:TextBox ID="txtConfirmPasswordStudent" runat="server" CssClass="form-control" TextMode="Password" />
        <br />
        <asp:Button ID="btnSaveStudent" runat="server" Text="Save Student Info" CssClass="btn-custom-style yes" OnClick="btnSaveStudent_Click" />
        <asp:Button ID="btnGoToProgress" runat="server" Text="Edit Student Progress" CssClass="btn-custom-style" OnClick="btnGoToProgress_Click" Style="margin-left: 10px;" />
    </asp:Panel>

    <!-- ADMIN PANEL -->
    <asp:Panel ID="pnlAdmin" runat="server" Visible="false">
        <h3>Admin Information</h3>

        <label for="txtAdminEmail">Email:</label>
        <asp:TextBox ID="txtAdminEmail" runat="server" CssClass="form-control" />

        <h4>Change Password</h4>
        <label for="txtCurrentPasswordAdmin">Current Password:</label>
        <asp:TextBox ID="txtCurrentPasswordAdmin" runat="server" CssClass="form-control" TextMode="Password" />

        <label for="txtNewPassword">New Password:</label>
        <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password" />

        <label for="txtConfirmPassword">Confirm New Password:</label>
        <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" />
        <br />
        <asp:Button ID="btnSaveAdmin" runat="server" Text="Save Admin Info" CssClass="btn-custom-style yes" OnClick="btnSaveAdmin_Click" />
    </asp:Panel>

    <!-- FACULTY PANEL -->
    <asp:Panel ID="pnlFaculty" runat="server" Visible="false">
        <h3>Faculty Information</h3>

        <label for="txtFacultyEmail">Email:</label>
        <asp:TextBox ID="txtFacultyEmail" runat="server" CssClass="form-control" />

        <h4>Change Password</h4>
        <label for="txtCurrentPasswordFaculty">Current Password:</label>
        <asp:TextBox ID="txtCurrentPasswordFaculty" runat="server" CssClass="form-control" TextMode="Password" />

        <label for="txtNewPasswordFaculty">New Password:</label>
        <asp:TextBox ID="txtNewPasswordFaculty" runat="server" CssClass="form-control" TextMode="Password" />

        <label for="txtConfirmPasswordFaculty">Confirm New Password:</label>
        <asp:TextBox ID="txtConfirmPasswordFaculty" runat="server" CssClass="form-control" TextMode="Password" />
        <br />
        <asp:Button ID="btnSaveFaculty" runat="server" Text="Save Faculty Info" CssClass="btn-custom-style yes" OnClick="btnSaveFaculty_Click" />
    </asp:Panel>

    <!-- DEPARTMENT HEAD PANEL -->
    <asp:Panel ID="pnlDepartmentHead" runat="server" Visible="false">
        <h3>Department Head Information</h3>

        <label for="txtDepartmentHeadEmail">Email:</label>
        <asp:TextBox ID="txtDepartmentHeadEmail" runat="server" CssClass="form-control" />

        <h4>Change Password</h4>
        <label for="txtCurrentPasswordDepartmentHead">Current Password:</label>
        <asp:TextBox ID="txtCurrentPasswordDepartmentHead" runat="server" CssClass="form-control" TextMode="Password" />

        <label for="txtNewPasswordDepartmentHead">New Password:</label>
        <asp:TextBox ID="txtNewPasswordDepartmentHead" runat="server" CssClass="form-control" TextMode="Password" />

        <label for="txtConfirmPasswordDepartmentHead">Confirm New Password:</label>
        <asp:TextBox ID="txtConfirmPasswordDepartmentHead" runat="server" CssClass="form-control" TextMode="Password" />
        <br />
        <asp:Button ID="btnSaveDepartmentHead" runat="server" Text="Save Department Head Info"
            CssClass="btn-custom-style yes" OnClick="btnSaveDepartmentHead_Click" />
    </asp:Panel>

    <!-- OUTPUT MESSAGE -->
    <asp:Label ID="lblOutput" runat="server" CssClass="text-success" Style="margin-top: 20px; display:block;" />

</asp:Content>
