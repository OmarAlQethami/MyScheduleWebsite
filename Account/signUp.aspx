<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="signUp.aspx.cs" Inherits="MyScheduleWebsite.Account.signUp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="../styles/SignUpStyles.css" rel="stylesheet" />

    <div>
        <asp:Label ID="lblInfo" runat="server" 
            Text="This page is only for students. If you are a faculty member, please contact the Admin through the help center for privileges." 
            CssClass="info-label" ForeColor="Red"></asp:Label>

        <table class="style1">
            <tr><td colspan="2"></td></tr>
            <tr>
                <td class="style2"><strong>Sign Up</strong></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="style2">&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="style2">English First Name:</td>
                <td>
                    <asp:TextBox ID="txtFName" runat="server" autocomplete="off"></asp:TextBox>
                    <asp:Label ID="lblFNameError" runat="server" ForeColor="Red"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="style2">English Last Name:</td>
                <td>
                    <asp:TextBox ID="txtLName" runat="server" autocomplete="off"></asp:TextBox>
                    <asp:Label ID="lblLNameError" runat="server" ForeColor="Red"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="style2">Arabic First Name:</td>
                <td>
                    <asp:TextBox ID="txtArFName" runat="server" autocomplete="off"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style2">Arabic Last Name:</td>
                <td>
                    <asp:TextBox ID="txtArLName" runat="server" autocomplete="off"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style2">Email:</td>
                <td>
                    <asp:TextBox ID="txtEmail" runat="server" autocomplete="off"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style2">Username:</td>
                <td>
                    <asp:TextBox ID="txtUsername" runat="server"></asp:TextBox>
                    <asp:Label ID="lblUsernameError" runat="server" ForeColor="Red" style="margin-left: 5px;"></asp:Label>
                    <asp:Label ID="lblOutput" runat="server" ForeColor="Red" style="margin-top: 10px;"></asp:Label>          
                </td>
            </tr>
            <tr>
                <td class="style2">Password:</td>
                <td>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" />
                    <asp:Label ID="lblPasswordError" runat="server" ForeColor="Red"></asp:Label>
                    <button type="button" id="btnTogglePassword" onclick="togglePasswordVisibility()">Show</button>
                </td>
            </tr>
            <tr>
                <td class="style2">Confirm Password:</td>
                <td>
                    <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" />
                    <asp:Label ID="lblConfirmPasswordError" runat="server" ForeColor="Red"></asp:Label>
                    <button type="button" id="btnToggleConfirmPassword" onclick="toggleConfirmPasswordVisibility()">Show</button>
                </td>
            </tr>
            <tr>
                <td class="style2">&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="style2">Student's University ID:</td>
                <td>
                    <asp:TextBox ID="txtUniId" runat="server"></asp:TextBox>
                    <asp:Label ID="lblUniIdError" runat="server" ForeColor="Red"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="style2">University:</td>
                <td>
                    <asp:DropDownList ID="ddlUniversity" runat="server" DataSourceID="SqlDataSource1" 
                        DataTextField="universityEnglishName" DataValueField="universityEnglishName" 
                        OnSelectedIndexChanged="ddlUniversity_SelectedIndexChanged" AutoPostBack="false" 
                        AppendDataBoundItems="True">
                        <asp:ListItem Text="Choose a University" Value="" Selected="True"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:Label ID="lblUniversityError" runat="server" ForeColor="Red"></asp:Label>
                    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                        ConnectionString="<%$ ConnectionStrings:MyScheduleWebsiteConStr %>" 
                        SelectCommand="SELECT [universityEnglishName] FROM [universities]">
                    </asp:SqlDataSource>
                </td>
            </tr>
            <tr>
                <td class="style2">Major:</td>
                <td>
                    <asp:DropDownList ID="ddlMajors" runat="server" OnSelectedIndexChanged="ddlMajors_SelectedIndexChanged" AutoPostBack="false">
                        <asp:ListItem Text="Choose a Major" Value="0" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="Computer Science" Value="1"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:Label ID="lblMajorError" runat="server" ForeColor="Red"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="style2">Current Level:</td>
                <td>
                    <asp:DropDownList ID="ddlCurrentLevel" runat="server">
                        <asp:ListItem Text="Choose your current level" Value="0" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="1" Value="1"></asp:ListItem>
                        <asp:ListItem Text="2" Value="2"></asp:ListItem>
                        <asp:ListItem Text="3" Value="3"></asp:ListItem>
                        <asp:ListItem Text="4" Value="4"></asp:ListItem>
                        <asp:ListItem Text="5" Value="5"></asp:ListItem>
                        <asp:ListItem Text="6" Value="6"></asp:ListItem>
                        <asp:ListItem Text="7" Value="7"></asp:ListItem>
                        <asp:ListItem Text="8" Value="8"></asp:ListItem>
                        <asp:ListItem Text="9" Value="9"></asp:ListItem>
                        <asp:ListItem Text="10" Value="10"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="style2">&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="style2">&nbsp;</td>
                <td>
                    <asp:Button ID="btnSignUp" runat="server" ForeColor="#0000FF" style="font-weight: bold" 
                        onclick="btnSignUp_Click" Text="Sign Up" 
                        OnClientClick="return confirm('Are all your information correct?')" />
                </td>
            </tr>
            <tr>
                <td class="style2">&nbsp;</td>
                <td>
                    <asp:Button ID="btnBack" runat="server" Text="Back" PostBackUrl="~/default.aspx" 
                        ForeColor="#000000" style="font-weight: bold" />
                </td>
            </tr>
            <tr>
                <td class="style2">&nbsp;</td>
                <td><asp:Label ID="Label1" runat="server" ForeColor="Red"></asp:Label></td>
            </tr>
        </table>
        <br />
    </div>

    <script type="text/javascript">
        function togglePasswordVisibility() {
            var passwordField = document.getElementById('<%= txtPassword.ClientID %>');
            var toggleButton = document.getElementById('btnTogglePassword');
            if (passwordField.type === "password") {
                passwordField.type = "text";
                toggleButton.innerHTML = "Hide";
            } else {
                passwordField.type = "password";
                toggleButton.innerHTML = "Show";
            }
        }

        function toggleConfirmPasswordVisibility() {
            var confirmPasswordField = document.getElementById('<%= txtConfirmPassword.ClientID %>');
            var toggleButton = document.getElementById('btnToggleConfirmPassword');
            if (confirmPasswordField.type === "password") {
                confirmPasswordField.type = "text";
                toggleButton.innerHTML = "Hide";
            } else {
                confirmPasswordField.type = "password";
                toggleButton.innerHTML = "Show";
            }
        }
    </script>

</asp:Content>