<%@ Page Title="Sign Up" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="signUp.aspx.cs" Inherits="MyScheduleWebsite.Account.signUp" %>

<%@ Register Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">


    <link href="../styles/SignUpStyles.css" rel="stylesheet" />

    <div class="container mt-5">
        <div class="row justify-content-center">

            <div class="col-lg-8 col-md-10 fill">

                <div class="card shadow-lg border-0 rounded-lg">

                    <div class="card-header text-white text-center rounded-top pd4">
                        <h3 class="card-title mb-0 py-3">Create Student Account</h3>
                    </div>

                    <div class="card-body p-4">
                        <div class="container mt-4">
                            <div class="row">
                                <div class="col-12" style="text-align: justify">
                                    <asp:Label 
                                        ID="lblInfo" 
                                        runat="server" 
                                        Text="This page is for student sign-ups only. Faculty, please contact Admin for account privileges." 
                                        CssClass="alert alert-warning d-block mb-3" 
                                        Visible="true">
                                    </asp:Label>
                                </div>
                            </div>
                            <div class="row" style="margin-top: 30px;">
                                <div class="col-12">
                                    <asp:Label 
                                        ID="lblOutput" 
                                        runat="server" 
                                        CssClass="alert alert-danger d-block mb-3" 
                                        Visible="false">
                                    </asp:Label>
                                </div>
                            </div>
                        </div>


                        
                        <hr class="mb-4" />

                        <asp:Panel ID="formPanel" runat="server" CssClass="mil">

                            <div class="row mb-3">
                                <div class="col-md-6">
                                    <div class="form-floating mb-3">
                                        <asp:TextBox ID="txtFName" runat="server" CssClass="form-control" placeholder="" autocomplete="off"></asp:TextBox>
                                        <label for="txtFName">English First Name</label>
                                        <asp:RequiredFieldValidator ID="rfvFName" runat="server" ControlToValidate="txtFName" ErrorMessage="First name is required." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="revFName" runat="server" ControlToValidate="txtFName" ErrorMessage="Must be in English." ValidationExpression="^[a-zA-Z\s]+$" CssClass="text-danger" Display="Dynamic"></asp:RegularExpressionValidator>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-floating mb-3">
                                        <asp:TextBox ID="txtLName" runat="server" CssClass="form-control" placeholder="" autocomplete="off"></asp:TextBox>
                                        <label for="txtLName">English Last Name</label>
                                        <asp:RequiredFieldValidator ID="rfvLName" runat="server" ControlToValidate="txtLName" ErrorMessage="Last name is required." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="revLName" runat="server" ControlToValidate="txtLName" ErrorMessage="Must be in English." ValidationExpression="^[a-zA-Z\s]+$" CssClass="text-danger" Display="Dynamic"></asp:RegularExpressionValidator>
                                    </div>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <div class="col-md-6">
                                    <div class="form-floating mb-3">
                                        <asp:TextBox ID="txtArFName" runat="server" CssClass="form-control" placeholder="" autocomplete="off"></asp:TextBox>
            <label for="txtArFName">Arabic First Name</label>
            <asp:RequiredFieldValidator ID="rfvArFName" runat="server" ControlToValidate="txtArFName" ErrorMessage="First name in Arabic is required." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="revArFName" runat="server" ControlToValidate="txtArFName" ErrorMessage="The name must be in Arabic only." ValidationExpression="^[\u0600-\u06FF\s]+$" CssClass="text-danger" Display="Dynamic"></asp:RegularExpressionValidator>
                                    </div>
                                </div>
                                <div class="col-md-6">
    <div class="form-floating mb-3">
        <asp:TextBox ID="txtArLName" runat="server" CssClass="form-control" placeholder="" autocomplete="off"></asp:TextBox>
        <label for="txtArLName">Arabic Last Name</label>
        <asp:RequiredFieldValidator ID="rfvArLName" runat="server" ControlToValidate="txtArLName" ErrorMessage="اLast name required in Arabic." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ID="revArLName" runat="server" ControlToValidate="txtArLName" ErrorMessage="The name must be in Arabic only." ValidationExpression="^[\u0600-\u06FF\s]+$" CssClass="text-danger" Display="Dynamic"></asp:RegularExpressionValidator>
    </div>
</div>
                            </div>

                            <div class="form-floating mb-3">
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="" autocomplete="off" TextMode="Email"></asp:TextBox>
                                <label for="txtEmail">Email Address</label>
                                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email is required." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Invalid email format." ValidationExpression="^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$" CssClass="text-danger" Display="Dynamic"></asp:RegularExpressionValidator>
                            </div>

                            <div class="form-floating mb-3">
                                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="" autocomplete="off" oninput="removeSpaces(this)"></asp:TextBox>
                                <label for="txtUsername">Username</label>
                                <asp:RequiredFieldValidator ID="rfvUsername" runat="server" ControlToValidate="txtUsername" ErrorMessage="Username is required." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revUsername" runat="server" ControlToValidate="txtUsername" ErrorMessage="Username cannot contain spaces." ValidationExpression="^\S+$" CssClass="text-danger" Display="Dynamic"></asp:RegularExpressionValidator>    </div>

                            <div class="row mb-3">
                                <div class="col-md-6">
                                    <div class="form-floating mb-3">
                                        <div class="input-group">
                                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" placeholder="Password" TextMode="Password"></asp:TextBox>
                                            <button class="btn btn-outline-secondary" type="button" onclick="togglePasswordVisibility()">
                                                <i id="passwordEyeIcon" class="fas fa-eye"></i>
                                            </button>
                                        </div>
                                        <label for="txtPassword">Password</label>
                                        <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword" ErrorMessage="Password is required." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="revPassword" runat="server" ControlToValidate="txtPassword" ErrorMessage="Min 4 characters." ValidationExpression=".{4,}" CssClass="text-danger" Display="Dynamic"></asp:RegularExpressionValidator>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-floating mb-3">
                                        <div class="input-group">
                                            <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" placeholder="Confirm Password" TextMode="Password"></asp:TextBox>
                                            <button class="btn btn-outline-secondary" type="button" onclick="toggleConfirmPasswordVisibility()">
                                                <i id="confirmPasswordEyeIcon" class="fas fa-eye"></i>
                                            </button>
                                        </div>
                                        <label for="txtConfirmPassword">Confirm Password</label>
                                        <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword" ErrorMessage="Confirm password is required." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="cvConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword" ControlToCompare="txtPassword" ErrorMessage="Passwords do not match." CssClass="text-danger" Display="Dynamic"></asp:CompareValidator>
                                    </div>
                                </div>
                            </div>

                            <div class="form-floating mb-3">
                                <asp:TextBox ID="txtUniId" runat="server" CssClass="form-control" placeholder="" autocomplete="off"></asp:TextBox>
                                <label for="txtUniId">University ID</label>
                                <asp:RequiredFieldValidator ID="rfvUniId" runat="server" ControlToValidate="txtUniId" ErrorMessage="University ID is required." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                            </div>

                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="form-floating mb-3">
                                        <asp:DropDownList ID="ddlUniversity" runat="server" CssClass="form-select" DataTextField="universityEnglishName" DataValueField="universityEnglishName" AutoPostBack="true" OnSelectedIndexChanged="ddlUniversity_SelectedIndexChanged">
                                            <asp:ListItem Text="Choose a University" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <label for="ddlUniversity">University</label>
                                        <asp:RequiredFieldValidator ID="rfvUniversity" runat="server" ControlToValidate="ddlUniversity" ErrorMessage="University is required." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </div>

                                    <div class="form-floating mb-3">
                                        <asp:DropDownList ID="ddlMajors" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlMajors_SelectedIndexChanged">
                                            <asp:ListItem Text="Choose a Major" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <label for="ddlMajors">Major</label>
                                        <asp:RequiredFieldValidator ID="rfvMajor" runat="server" ControlToValidate="ddlMajors" ErrorMessage="Major is required." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </div>

                                    <div class="form-floating mb-3">
                                        <asp:DropDownList ID="ddlCurrentLevel" runat="server" CssClass="form-select">
                                            <asp:ListItem Text="Choose your current level" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                        <label for="ddlCurrentLevel">Current Level</label>
                                        <asp:RequiredFieldValidator ID="rfvCurrentLevel" runat="server" ControlToValidate="ddlCurrentLevel" ErrorMessage="Current level is required." CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>

                            <div class="d-grid mt-4 mb-0 gi">
                                <asp:Button ID="btnCancel" runat="server" CausesValidation="false" CssClass="btn btn-secondary btn-lg" PostBackUrl="~/default.aspx" Text="Back" />
                                <asp:Button ID="btnSignUp" runat="server" CssClass="btn btn-primary btn-lg" Text="Sign Up"  OnClick="btnSignUp_Click" OnClientClick="return confirmSignUp();" />
                            </div>
                        </asp:Panel>

    </div>

    <div class="card-footer text-center py-3">
                        <div class="small">Already have an account? <a href="login.aspx">Log in</a></div>
                    </div>

                </div>

            </div>

        </div>

    </div>

    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.0/js/bootstrap.bundle.min.js"></script>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css">

    <script type="text/javascript">
        function togglePasswordVisibility() {
            var passwordField = document.getElementById('<%= txtPassword.ClientID %>');
            var icon = document.getElementById('passwordEyeIcon');
            if (passwordField.type === "password") {
                passwordField.type = "text";
                icon.classList.replace('fa-eye', 'fa-eye-slash');
            } else {
                passwordField.type = "password";
                icon.classList.replace('fa-eye-slash', 'fa-eye');
            }
        }

        function toggleConfirmPasswordVisibility() {
            var confirmPasswordField = document.getElementById('<%= txtConfirmPassword.ClientID %>');
            var icon = document.getElementById('confirmPasswordEyeIcon');
            if (confirmPasswordField.type === "password") {
                confirmPasswordField.type = "text";
                icon.classList.replace('fa-eye', 'fa-eye-slash');
            } else {
                confirmPasswordField.type = "password";
                icon.classList.replace('fa-eye-slash', 'fa-eye');
            }
        }
    </script>
    <script type="text/javascript">
        function confirmSignUp() {
            return confirm("Are you sure you want to register?");
        }
    </script>
    <script type="text/javascript">
        function removeSpaces(input) {
            input.value = input.value.replace(/\s/g, '');
        }
    </script>
</asp:Content>
