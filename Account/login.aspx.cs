using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Configuration;
using MyScheduleWebsite.App_Code;

namespace MyScheduleWebsite.Account
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Username"] != null)
            {
                if (Roles.IsUserInRole(Session["Username"].ToString(), "student"))
                {
                    Response.Redirect("~/StudentPage.aspx");
                }
                else if (Roles.IsUserInRole(Session["Username"].ToString(), "faculty"))
                {
                    Response.Redirect("~/FacultyPage.aspx");
                }
                else if (Roles.IsUserInRole(Session["Username"].ToString(), "admin"))
                {
                    Response.Redirect("~/admin/AdminPage.aspx");
                }
            }
            createAdminAndUserByDefault();
        }

         private void createAdminAndUserByDefault()
        {
            try
            {
                string strExistingAdmin = "";
                string strAppName = "/MyScheduleWebsite";
                string strAdminUserName = "Admin";  
                string strAdminPassword = "zxcvbnm999"; 
                string strRoleName = "admin";
                string strRoleFaculty = "faculty";
                string strRoleStudent = "student";
                string strEmail = "Admin@MySchedule.com";
                strAppName = Membership.ApplicationName.ToString();
                CRUD myCrud = new CRUD();
                Dictionary<string, object> myPara = new Dictionary<string, object>();
                myPara.Add("@userName", strAdminUserName);
                myPara.Add("@appName", strAppName);
                strExistingAdmin = myCrud.checkUserExist("p_doesUserExist", myPara);
                if (strExistingAdmin.Length == 0)
                {
                    if (!Roles.RoleExists(strRoleName))
                        Roles.CreateRole(strRoleName);
                    if (!Roles.RoleExists(strRoleFaculty))
                        Roles.CreateRole(strRoleFaculty);
                    if (!Roles.RoleExists(strRoleStudent))
                        Roles.CreateRole(strRoleStudent);
                    if (!Membership.ValidateUser(strAdminUserName, strAdminPassword))
                    {
                        Membership.CreateUser(strAdminUserName, strAdminPassword, strEmail);
                        if (!Roles.IsUserInRole(strAdminUserName, strRoleName))
                            Roles.AddUserToRole(strAdminUserName, strRoleName);
                    }
                }
                if (strExistingAdmin.Length >= 0)
                {
                    if (!Roles.RoleExists(strRoleName))
                        Roles.CreateRole(strRoleName);
                    if (!Membership.ValidateUser(strAdminUserName, strAdminPassword))
                    {
                        Membership.DeleteUser(strAdminUserName);
                        Membership.CreateUser(strAdminUserName, strAdminPassword, strEmail);
                        if (!Roles.IsUserInRole(strAdminUserName, strRoleName))
                            Roles.AddUserToRole(strAdminUserName, strRoleName);
                    }
                }
            }
            catch (Exception ex)
            {
                lblOutput.Text = ex.Message.ToString();
                
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Session["Username"] = txtUserName.Text;
            bool blnAuthenticate = AuthenticateUser();

            if (blnAuthenticate)
            {
                FormsAuthentication.RedirectFromLoginPage(txtUserName.Text, false);

                if (Roles.IsUserInRole(txtUserName.Text, "student"))
                {
                    Response.Redirect("~/StudentPage.aspx");
                }
                else if (Roles.IsUserInRole(txtUserName.Text, "faculty"))
                {
                    Response.Redirect("~/FacultyPage.aspx");
                }
                else if (Roles.IsUserInRole(txtUserName.Text, "admin"))
                {
                    Response.Redirect("~/admin/AdminPage.aspx");
                }
            }
            else
            {
                lblOutput.Text = "Your login was invalid, please try again.";
            }
        }


        protected bool AuthenticateUser()
        {
            string userName = txtUserName.Text;
            string password = txtPassword.Text;
            bool userFound = false;
            try
            {
                userFound = Membership.ValidateUser(userName, password);
                lblOutput.Text = Session["Username"].ToString();
            }
            catch
            {
                lblOutput.Text = "Your login was invalid, please try again.";
            }
            return userFound;
        }

        protected bool Authenticate(Dictionary<string, object> userNamePassword)
        {
            string mySql = "SELECT userId,userName,password,email  FROM myUsers where userName =@userName and password=@passWord";
            CRUD myCrud = new CRUD();
            bool userFound = myCrud.authenticateUser(mySql, userNamePassword);
            return userFound;
        }
    }
}