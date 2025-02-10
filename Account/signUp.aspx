<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="signUp.aspx.cs" Inherits="MyScheduleWebsite.Account.signUp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="/styles/SignUpStyles.css">

    <div>
    
        <table class="style1">
            <tr><td colspan="2"></td></tr>
            <tr>
                <td class="style2">
                    <strong>Sign Up</strong></td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="style2">
                    &nbsp;</td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="style2">
                    English First Name:</td>
                <td>
                    <asp:TextBox ID="txtFName" runat="server" autocomplete="off"></asp:TextBox>
  
                </td>
            </tr>
            <tr>
                <td class="style2">
                    English
                    Last Name:</td>
                <td>
                    <asp:TextBox ID="txtLName" runat="server" autocomplete="off"></asp:TextBox>
  
                </td>
            </tr>
            <tr>
                <td class="style2">
                    Arabic First Name:</td>
                <td>
                    <asp:TextBox ID="txtArFName" runat="server" autocomplete="off"></asp:TextBox>
  
                </td>
            </tr>
            <tr>
                <td class="style2">
                    Arabic Last Name:</td>
                <td>
                    <asp:TextBox ID="txtArLName" runat="server" autocomplete="off"></asp:TextBox>
  
                </td>
            </tr>
            <tr>
                <td class="style2">
                    Email:</td>
                <td>
                    <asp:TextBox ID="txtEmail" runat="server" autocomplete="off"></asp:TextBox>
                    
  
                </td>
            </tr>
            <tr>
                <td class="style2">
                    Username:</td>
                <td>
                    <asp:TextBox ID="txtUsername" runat="server" autocomplete="off"></asp:TextBox>
  
                </td>
            </tr>
            <tr>
                <td class="style2">
                    Password:</td>
                <td>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox> 
                </td>
            </tr>
            <tr>
                <td class="style2">
                    &nbsp;</td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="style2">
                    Student&#39;s
                    University ID:</td>
                <td>
                    <asp:TextBox ID="txtUniId" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="style2">
                    University:</td>
                <td>
                    <asp:DropDownList ID="ddlUniversity" runat="server" DataSourceID="SqlDataSource1" DataTextField="universityEnglishName" DataValueField="universityEnglishName" OnSelectedIndexChanged="ddlUniversity_SelectedIndexChanged" AutoPostBack="True" AppendDataBoundItems="True">
                        <asp:ListItem Text="Choose a University" Value="" Selected="True"></asp:ListItem>
                    </asp:DropDownList>

                    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                        ConnectionString="<%$ ConnectionStrings:MyScheduleWebsiteConStr %>" 
                        SelectCommand="SELECT [universityEnglishName] FROM [universities]">
                    </asp:SqlDataSource>
                </td>

            </tr>
            <tr>
                <td class="style2">
                    Major:</td>
                <td>
                    <asp:DropDownList ID="ddlMajors" runat="server" OnSelectedIndexChanged="ddlMajors_SelectedIndexChanged" AutoPostBack="True">
                        <asp:ListItem Text="Choose a Major" Value="" Selected="True"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="style2">
                    Current Level:</td>
                <td>
                    <asp:DropDownList ID="ddlCurrentLevel" runat="server">
                        <asp:ListItem Text="Choose your current level" Value="" Selected="True"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="style2">
                    &nbsp;</td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="style2">
                    &nbsp;</td>
                <td>
                    <asp:Button ID="btnSignUp" runat="server" ForeColor="#0000FF" style="font-weight: bold" onclick="btnSignUp_Click" 
                        Text="Sign Up" OnClientClick="return confirm('Are all your information correct?')"/>
                </td>
            </tr>
            <tr>
                <td class="style2">
                    &nbsp;</td>
                <td>
                    <asp:Button ID="btnBack" runat="server" Text="Back" PostBackUrl="~/default.aspx" ForeColor="#000000" style="font-weight: bold"/>
                </td>
            </tr>
            <tr>
                <td class="style2">
                    &nbsp;</td>
                <td>
                    <asp:Label ID="lblOutput" runat="server"></asp:Label>
                </td>
            </tr>
        </table>
        
        <br />
        
    
    </div>

</asp:Content>
