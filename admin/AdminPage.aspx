<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminPage.aspx.cs" 
    Inherits="MyScheduleWebsite.admin.AdminPage" EnableEventValidation="false" ValidateRequest="false"
%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="/styles/AdminStyles.css">

    <asp:Label ID="lblSignUpOutput" runat="server"></asp:Label>
    <asp:Label ID="lblSignUpOutput1" runat="server"></asp:Label>
    <asp:Label ID="lblMajorsOutput" runat="server"></asp:Label>
    
    

    <div class="tab-container">
        <div class="tab-header">
            <button class="tab-button active" onclick="openTab(event, 'roleManagement')">User Role Management</button>
            <button class="tab-button" onclick="openTab(event, 'facultySignup')">Faculty Member Sign Up</button>
            <button class="tab-button" onclick="openTab(event, 'deptHeadSignup')">Department Head Sign Up</button>
            <button class="tab-button" onclick="openTab(event, 'universityRegister')">University Registration</button>
        </div>
        
        <!-- User Role Management Tab -->
        <div id="roleManagement" class="tab-content active">
            <h3> User Role Management Console</h3>
            <br />
            <div class="admin-container">
                <table border="1">
                    <tr>
                        <th>Roles</th>
                        <th>Users</th>
                        <th>
                            <asp:Button ID="btnUserRoleAssign" runat="server" 
                            Text="Link User Role" OnClick="btnUserRoleAssign_Click" />
                        </th>
                    </tr>
                    <tr>
                        <td class="top-align">
                            <asp:CheckBoxList ID="cBLRoles" runat="server"></asp:CheckBoxList>
                        </td>
                         <td class="top-align">
                            <asp:CheckBoxList ID="cBLUsers" runat="server"></asp:CheckBoxList>
                        </td>
                         <td class="top-align">
                            <asp:Button ID="btnUnlinkUserRoles" runat="server" OnClick="btnUnlinkUserRoles_Click" Text="Unlink User Role" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">&nbsp;</td>
                        <td>
                             <asp:Button ID="btnDeleteRoles" runat="server" OnClick="btnDeleteRoles_Click" Text="Delete Roles" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">&nbsp;</td>
                        <td>
                            <asp:Button ID="btnDeleteUsers" runat="server" OnClick="btnDeleteUsers_Click" Text="Delete Users" />
                        </td>
                    </tr>
                 </table>
            </div>

            <table class="admin-container">
                <tr>
                    <td colspan="5" align="center">
                        <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>User</td>
                    <td align="center" class="style4">
                        <asp:TextBox ID="txtUser" runat="server" autocomplete="off"></asp:TextBox>
                    </td>
                    <td align="center" class="style4" style="width: 42px">Role</td>
                    <td align="center" class="style4" style="width: 61px">
                        <asp:TextBox ID="txtRole" runat="server" autocomplete="off"></asp:TextBox>
                    </td>
                    <td style="width: 177px">
                        <asp:Button ID="btnLinkUserRole" runat="server" OnClick="btnLinkUserRole_Click" Text="Link User Role"
                            Width="124px" style="margin-left: 0px" TabIndex="6" />
                    </td>
                </tr>
                <tr>
                    <td>Password</td>
                    <td>
                        <asp:TextBox ID="txtPassword" runat="server" TabIndex="1" autocomplete="off"></asp:TextBox>
                    </td>
                    <td></td>
                    <td></td>
                    <td style="width: 177px">
                        <asp:Button ID="btnUnLinkUserToRole" runat="server" OnClick="btnUnLinkUserToRole_Click"
                            Text="UnlinkUsertoRole" Width="124px" />
                    </td>
                </tr>
                <tr>
                    <td class="style3" style="width: 68px">Email</td>
                    <td align="center" class="style4">
                        <asp:TextBox ID="txtEmail" runat="server" TabIndex="2" autocomplete="off"></asp:TextBox>
                    </td>
                    <td align="center" class="style4" style="width: 42px">&nbsp;</td>
                    <td align="center">&nbsp;</td>
                    <td>
                        <asp:Button ID="btnShowAllUser" runat="server" OnClick="btnShowAllUser_Click" Text="Show All Users"
                            Width="124px" />
                    </td>
                </tr>
                <tr>
                    <td class="style3" style="width: 68px">&nbsp;</td>
                    <td align="center" class="style4">
                        <asp:Button ID="btnCreateUser0" runat="server" OnClick="btnCreateUser_Click" Style="margin-left: 0px"
                            Text="Create User" TabIndex="3" />
                    </td>
                    <td align="center" class="style4" style="width: 42px">&nbsp;</td>
                    <td align="center">
                        <asp:Button ID="btnCreateRole0" runat="server" OnClick="btnCreateRole_Click" 
                            Text="Create Role" Width="98px" TabIndex="5" />
                    </td>
                    <td>
                        <asp:Button ID="btnShowAllRoles" runat="server" OnClick="btnShowAllRoles_Click" 
                            Text="Show All Roles" Width="124px"/>
                    </td>
                </tr>
                <tr>
                    <td class="style3">&nbsp;</td>
                    <td align="center" class="style4">
                        <asp:Button ID="btnDeleteUser0" runat="server" OnClick="btnDeleteUser_Click" Text="Delete User" Width="105px" />
                    </td>
                    <td align="center" class="style4" style="width: 42px"></td>
                    <td align="center">
                        <asp:Button ID="btnDeleteRole1" runat="server" OnClick="btnDeleteRole_Click" Text="Delete Role" />
                    </td>
                    <td style="width: 177px">
                        <asp:Button ID="btnUpdateUser" runat="server" OnClick="btnUpdateUser_Click" Text="Update User" Width="125px" />
                    </td>
                </tr>
            </table>
        </div>
        
        <!-- Faculty Member Sign Up Tab -->
        <div id="facultySignup" class="tab-content">
            <h3>Sign Up a new Faculty Member:</h3>
            <div class="form-container">
                <div class="input-group">
                    <asp:TextBox ID="txtFName" runat="server" CssClass="asp-input" placeholder=" "></asp:TextBox>
                    <label class="floating-label" for="txtFName">English First Name</label>
                </div>

                <div class="input-group">
                    <asp:TextBox ID="txtLName" runat="server" CssClass="asp-input" placeholder=" "></asp:TextBox>
                    <label class="floating-label" for="txtLName">English Last Name</label>
                </div>

                <div class="input-group">
                    <asp:TextBox ID="txtArFName" runat="server" CssClass="asp-input" placeholder=" "></asp:TextBox>
                    <label class="floating-label" for="txtArFName">Arabic First Name</label>
                </div>

                <div class="input-group">
                    <asp:TextBox ID="txtArLName" runat="server" CssClass="asp-input" placeholder=" "></asp:TextBox>
                    <label class="floating-label" for="txtArLName">Arabic Last Name</label>
                </div>

                <div class="input-group">
                    <asp:TextBox ID="txtEmail1" runat="server" CssClass="asp-input" placeholder=" " TextMode="Email"></asp:TextBox>
                    <label class="floating-label" for="txtEmail1">Email</label>
                </div>

                <div class="input-group">
                    <asp:TextBox ID="txtUsername1" runat="server" CssClass="asp-input" placeholder=" "></asp:TextBox>
                    <label class="floating-label" for="txtUsername1">Username</label>
                </div>

                <div class="input-group">
                    <asp:TextBox ID="txtPassword1" runat="server" CssClass="asp-input" placeholder=" " TextMode="Password"></asp:TextBox>
                    <label class="floating-label" for="txtPassword1">Password</label>
                </div>

                <div>
                    <label for="ddlUniversity">University</label>
                    <br />
                    <asp:DropDownList ID="ddlUniversity" runat="server" DataSourceID="SqlDataSource1" DataTextField="universityEnglishName" DataValueField="universityEnglishName" OnSelectedIndexChanged="ddlUniversity_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="True">
                        <asp:ListItem Text="Choose a University" Value="" Selected="True"></asp:ListItem>
                    </asp:DropDownList>
                    
                    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                        ConnectionString="<%$ ConnectionStrings:MyScheduleWebsiteConStr %>" 
                        SelectCommand="SELECT [universityEnglishName] FROM [universities]">
                    </asp:SqlDataSource>
                </div>

                <div>
                    <label for="ddlMajors">Major</label>
                    <br />
                    <asp:DropDownList ID="ddlMajors" runat="server" AutoPostBack="True">
                        <asp:ListItem Text="Choose a Major " Value=" " Selected="True"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <br /><br />

                <asp:Button ID="btnSignUp" runat="server" CssClass="button" Text="Sign Up" 
                    OnClick="btnSignUp_Click" OnClientClick="return confirm('Are all your information correct?');" />
            </div>
        </div>
        
        <!-- Department Head Sign Up Tab -->
        <div id="deptHeadSignup" class="tab-content">
            <h3>Sign Up a new Department Head Member:</h3>
            <div class="form-container">
                <div class="input-group">
                    <asp:TextBox ID="txtFName1" runat="server" CssClass="asp-input" placeholder=" " autocomplete="off"></asp:TextBox>
                    <label class="floating-label" for="txtFName1">English First Name</label>
                </div>

                <div class="input-group">
                    <asp:TextBox ID="txtLName1" runat="server" CssClass="asp-input" placeholder=" " autocomplete="off"></asp:TextBox>
                    <label class="floating-label" for="txtLName1">English Last Name</label>
                </div>

                <div class="input-group">
                    <asp:TextBox ID="txtArFName1" runat="server" CssClass="asp-input" placeholder=" " autocomplete="off"></asp:TextBox>
                    <label class="floating-label" for="txtArFName1">Arabic First Name</label>
                </div>

                <div class="input-group">
                    <asp:TextBox ID="txtArLName1" runat="server" CssClass="asp-input" placeholder=" " autocomplete="off"></asp:TextBox>
                    <label class="floating-label" for="txtArLName1">Arabic Last Name</label>
                </div>

                <div class="input-group">
                    <asp:TextBox ID="txtEmail2" runat="server" CssClass="asp-input" placeholder=" " autocomplete="off"></asp:TextBox>
                    <label class="floating-label" for="txtEmail2">Email</label>
                </div>

                <div class="input-group">
                    <asp:TextBox ID="txtUserName2" runat="server" CssClass="asp-input" placeholder=" " autocomplete="off"></asp:TextBox>
                    <label class="floating-label" for="txtUserName2">Username</label>
                </div>

                <div class="input-group">
                    <asp:TextBox ID="txtPass1" runat="server" CssClass="asp-input" placeholder=" " TextMode="Password"></asp:TextBox>
                    <label class="floating-label" for="txtPass1">Password</label>
                </div>
                <br />

                <div class="input-group">
                    <label for="ddlUniversity1">University</label>
                    <br />
                    <asp:DropDownList ID="ddlUniversity1" runat="server" DataSourceID="SqlDataSource2" 
                        DataTextField="universityEnglishName" DataValueField="universityEnglishName" 
                        OnSelectedIndexChanged="ddlUniversity1_SelectedIndexChanged" AutoPostBack="True" 
                        AppendDataBoundItems="True">
                        <asp:ListItem Text="Choose a University" Value="" Selected="True"></asp:ListItem>
                    </asp:DropDownList>
                
                    <asp:SqlDataSource ID="SqlDataSource2" runat="server" 
                        ConnectionString="<%$ ConnectionStrings:MyScheduleWebsiteConStr %>" 
                        SelectCommand="SELECT [universityEnglishName] FROM [universities]">
                    </asp:SqlDataSource>
                </div>

                <div class="input-group">
                    <label for="ddlMajors1">Major</label> 
                    <br />
                    <asp:DropDownList ID="ddlMajors1" runat="server" AutoPostBack="True">
                        <asp:ListItem Text="Choose a Major" Value="" Selected="True"></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <asp:Button ID="btnSignUp1" runat="server" CssClass="button" Text="Sign Up" 
                    onclick="btnSignUp1_Click" 
                    OnClientClick="return confirm('Are all your information correct?')"/>
            </div>
        </div>
        
        <!-- University Registration Tab -->
        <div id="universityRegister" class="tab-content">
            <h3>Register a new University:</h3>
            <div class="form-container">
                <div class="input-group">
                    <asp:TextBox ID="txtUName" runat="server" CssClass="asp-input" placeholder=" " autocomplete="off"></asp:TextBox>
                    <label class="floating-label" for="txtUName">University English Name</label>
                </div>

                <div class="input-group">
                    <asp:TextBox ID="txtUArName" runat="server" CssClass="asp-input" placeholder=" " autocomplete="off"></asp:TextBox>
                    <label class="floating-label" for="txtUArName">University Arabic Name</label>
                </div>

                <div class="input-group">
                    <asp:TextBox ID="txtMajors" runat="server" CssClass="asp-input" placeholder=" "></asp:TextBox>
                    <label class="floating-label" for="txtMajors">Enter Majors</label>
                </div>

                <div class="input-group">
                    <asp:TextBox ID="txtArMajors" runat="server" CssClass="asp-input" placeholder=" "></asp:TextBox>
                    <label class="floating-label" for="txtArMajors">Enter Arabic Majors</label>
                </div>

                <div class="input-group">
                    <asp:TextBox ID="txtTotal" runat="server" CssClass="asp-input" placeholder=" " autocomplete="off"></asp:TextBox>
                    <label class="floating-label" for="txtTotal">Total Credit Hours</label>
                </div>

                <asp:Button ID="btnAddMajors" runat="server" CssClass="button" Text="Register" 
                    onclick="btnAddMajors_Click" 
                    OnClientClick="return confirm('Are all your information correct?')"/>
            </div>
        </div>
    </div>

   <script>
       // Improved Tab Functionality with Persistent State
       function openTab(evt, tabName) {
           try {
               // Prevent default behavior if event exists
               if (evt && evt.preventDefault) {
                   evt.preventDefault();
               }

               // Cache DOM elements
               const tabContents = document.querySelectorAll('.tab-content');
               const tabButtons = document.querySelectorAll('.tab-button');
               const targetTab = document.getElementById(tabName);
               const currentButton = evt?.currentTarget;

               // Validate elements exist
               if (!targetTab || !currentButton) {
                   console.error('Tab elements not found');
                   return;
               }

               // Hide all tabs and deactivate buttons
               tabContents.forEach(tab => {
                   tab.classList.remove('active');
                   tab.style.display = 'none';
               });

               tabButtons.forEach(btn => {
                   btn.classList.remove('active');
                   btn.setAttribute('aria-selected', 'false');
               });

               // Show target tab and activate button
               targetTab.classList.add('active');
               targetTab.style.display = 'block';
               currentButton.classList.add('active');
               currentButton.setAttribute('aria-selected', 'true');

               // Store active tab in session storage
               try {
                   sessionStorage.setItem('activeAdminTab', tabName);
               } catch (e) {
                   console.warn('Could not save tab state:', e);
               }

               // Dispatch custom event
               const tabChangedEvent = new CustomEvent('adminTabChanged', {
                   detail: { tabName }
               });
               window.dispatchEvent(tabChangedEvent);

           } catch (error) {
               console.error('Error in tab navigation:', error);
           }
       }

       // Initialize tabs on page load
       document.addEventListener('DOMContentLoaded', function () {
           try {
               // Restore active tab from session storage or default to first
               const savedTab = sessionStorage.getItem('activeAdminTab');
               const defaultTab = savedTab || 'roleManagement';

               // Find the button for the saved/default tab
               const targetButton = document.querySelector(`.tab-button[onclick*="${defaultTab}"]`);

               if (targetButton) {
                   // Create synthetic event
                   const event = {
                       currentTarget: targetButton,
                       preventDefault: () => { }
                   };
                   openTab(event, defaultTab);
               } else {
                   // Fallback: activate first tab if none found
                   const firstTab = document.querySelector('.tab-content');
                   const firstButton = document.querySelector('.tab-button');
                   if (firstTab && firstButton) {
                       firstTab.classList.add('active');
                       firstTab.style.display = 'block';
                       firstButton.classList.add('active');
                   }
               }
           } catch (error) {
               console.error('Tab initialization error:', error);
           }
       });
   </script>
</asp:Content>