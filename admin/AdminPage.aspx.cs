using MyScheduleWebsite.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MyScheduleWebsite.admin
{
    public partial class AdminPage : System.Web.UI.Page
    {
        string appName = "";
        string appNameWithProvider = "";

        private bool CheckUserExistance(string userName)
        {
            if (Membership.GetUser(userName) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        protected void postMsg(string msg)
        {
            lblMsg.Text = msg.ToString();
        }
        protected void postMsgClient(string msg)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert(msg)", true);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            appName = Membership.ApplicationName;
            appNameWithProvider = Membership.ApplicationName + "<BR>";
            appNameWithProvider += Membership.Provider + "<BR>";
            postMsg(appNameWithProvider);

            populateGvAllUsers();

            if (!Request.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
                Response.End();
            }


            if (!Page.IsPostBack)
            {
                populateCheckBoxListRolesUsers();
            }
        }
        public override void VerifyRenderingInServerForm(Control control)
        {

        }
        protected void populateCheckBoxListRolesUsers()
        {
            cBLUsers.DataSource = System.Web.Security.Membership.GetAllUsers();
            cBLUsers.DataBind();

            cBLRoles.DataSource = System.Web.Security.Roles.GetAllRoles();
            cBLRoles.DataBind();
        }
        void AssignDefaultRoleToUser(string userName, string NewRole)
        { 
            if (User.Identity.Name == userName)
            {
                if (!Roles.IsUserInRole(NewRole))
                {
                    Roles.AddUserToRole(User.Identity.Name, NewRole);
                    postMsg("Success");
                }
                else
                {
                    postMsg("User already in the role");
                }
            }
        }
        void addUserToRole(string user, string role)
        {
            if (!Roles.IsUserInRole(role))
                Roles.AddUserToRole(user, role);
            else
                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('User already in this role')", true);
            return;
        }
        protected void btnCreateRole_Click(object sender, EventArgs e)
        {
            if (!Roles.RoleExists(txtRole.Text))
            {
                createRole(txtRole.Text);
                populateGvAllUsers();

            }
            else
            {
                postMsg("Role already exists!!");
            }
            populateCheckBoxListRolesUsers();
        }
        void createRole(string myRole)
        {
            if (!Roles.RoleExists(myRole))
            {
                Roles.CreateRole(myRole);
                postMsg("New Role Created");
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('Role already Exists')", true);
                postMsg("Role already exists");
                return;
            }
        }
        protected void btnDeleteRole_Click(object sender, EventArgs e)
        {
            if (!Roles.RoleExists(txtRole.Text))
            {
                lblMsg.Text = "Role Does not Exist";
                return;
            }
            if (Roles.RoleExists(txtRole.Text) && Roles.GetUsersInRole(txtRole.Text).Length == 0)
            {
                Roles.DeleteRole(txtRole.Text);
                lblMsg.Text = "Role Deleted";
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('Role is in use by other users')", true);
                postMsg("Role is in use by other users");
            }
            populateCheckBoxListRolesUsers();
            populateGvAllUsers();
        }
        protected void btnCreateUser_Click(object sender, EventArgs e)
        {
            try
            {
                string vUser = txtUser.Text.ToString();
                string vPassword = txtPassword.Text.ToString();
                string vEmail = txtEmail.Text.ToString();
                if (!Membership.ValidateUser(vUser, vPassword))
                {
                    Membership.CreateUser(vUser, vPassword, vEmail);
                    postMsg("User Created Successfuly");
                    populateGvAllUsers();
                }

                else
                {
                    postMsg("User already exists!!");
                    return;
                }
                populateCheckBoxListRolesUsers();
            }

            catch (Exception ex)
            {
                postMsg(ex.Message.ToString());
            }
        }
        protected void btnLinkUserRole_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUser.Text) || string.IsNullOrEmpty(txtRole.Text))
            {
                lblMsg.Text = "Fill both User and Role fields";
                return;
            }
            
            if (Membership.FindUsersByName(txtUser.Text).Count == 0)
            {
                lblMsg.Text = "User Does not Exists";
                return;
            }
            if (!Roles.RoleExists(txtRole.Text))
            {
                lblMsg.Text = "Role Does not Exists";
                return;
            }
            if (Roles.RoleExists(txtRole.Text) && !Roles.IsUserInRole(txtUser.Text, txtRole.Text))
            {
                Roles.AddUserToRole(txtUser.Text, txtRole.Text);
                lblMsg.Text = "Linked Successful";
                populateGvAllUsers();
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('User already assigned a role')", true);
                postMsg("User already assigned a role");
                return;
            }
        }
        protected void btnShowAllUser_Click(object sender, EventArgs e)
        {
            
        }
        protected void btnShowAllRoles_Click(object sender, EventArgs e)
        {
            Page_Load(this, null);
        }
        protected void btnDeleteUser_Click(object sender, EventArgs e)
        {  
            MembershipUser existingUser = null;
            existingUser = Membership.GetUser(txtUser.Text);
            if (existingUser != null)
            {
                Guid userId = (Guid)Membership.GetUser(txtUser.Text).ProviderUserKey;

                CRUD myCrud = new CRUD();
                string mySql = @"DELETE FROM Customers WHERE UserId = @UserId";

                Dictionary<string, object> myPara = new Dictionary<string, object>();
                myPara.Add("@UserId", userId);
                int rtn = myCrud.InsertUpdateDelete(mySql, myPara);
                if (rtn >= 1)
                {
                    if (Membership.DeleteUser(txtUser.Text))
                    {
                        postMsg("Was Deleted");
                        populateGvAllUsers();
                    }
                    else
                    {
                        postMsg("Was Not Deleted");
                    }
                }
                else
                {
                    postMsg("Was Not Deleted from both tables");
                }
            }
            else
            {
                postMsg("User does not exists!!");
            }
            populateCheckBoxListRolesUsers();
        }
        protected void btnUnLinkUserToRole_Click(object sender, EventArgs e)
        {  
            if (Roles.IsUserInRole(txtUser.Text, txtRole.Text))
            {
                Roles.RemoveUserFromRole(txtUser.Text, txtRole.Text);
                lblMsg.Text = "Unlinked Successfully";

                populateGvAllUsers();
            }
            else
            {
                lblMsg.Text = "Unlinked Failed: User is not in the specified role";
                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('Unlinked Failed: User is not in the specified role')", true);
                return;
            }
        }
        
        protected void populateGvAllUsers()
        {
            
            string userCmd = "select username from dbo.aspnet_Users order by username";
            populateUserRole(userCmd, gvUsers);

            string roleCmd = "select rolename  from dbo.aspnet_Roles order by rolename";
            populateUserRole(roleCmd, gvRoles);

            string selectCommandNonANSI = @"select u.username ,r.rolename " +
                                "from dbo.aspnet_Users u, dbo.aspnet_Roles r  ,dbo.aspnet_UsersInRoles ur,dbo.aspnet_Applications  ap "
                              + "where u.UserId= ur.UserId  "
                              + "and ur.RoleId= r.RoleId "
                             + " and   applicationname ='" + appName + "'"
                                + "  order by username";
            populateUserRole(selectCommandNonANSI, gvNonAnsiInnerJoin);

            string innerJoin = @"select u.username ,r.rolename  "
                               + " from dbo.aspnet_Users u "
                               + " inner join dbo.aspnet_UsersInRoles ur on  u.UserId= ur.UserId   "
                               + " inner join   dbo.aspnet_Roles r   on ur.RoleId= r.RoleId  "
                               + " inner join dbo.aspnet_Applications  ap on u.applicationId= ap.applicationId"
                               + " where applicationname ='" + appName + "'"
                               + "  order by username";
            populateUserRole(innerJoin, gvInnerJoin);

            string leftOuterJoin = "select u.username ,r.rolename  "
                               + " from dbo.aspnet_Users u"
                               + " left outer join dbo.aspnet_UsersInRoles ur on  u.UserId= ur.UserId  "
                               + " left outer join   dbo.aspnet_Roles r   on ur.RoleId= r.RoleId  "
                               + " inner join dbo.aspnet_Applications  ap on u.applicationId= ap.applicationId"
                               + " where applicationname ='" + appName + "'"
                               + " order by username";
            populateUserRole(leftOuterJoin, gvLeftOuterJoin);

            string rightOuterJoin = "select distinct  u.username, r.rolename "
                               + " from dbo.aspnet_Applications ap inner join dbo.aspnet_Membership m on ap.applicationid=m.applicationid"
                               + " inner join dbo.aspnet_Users u on m.applicationid = u.applicationid "
                               + " right outer join dbo.aspnet_UsersInRoles ur on  u.UserId= ur.UserId   "
                               + " right outer join   dbo.aspnet_Roles r   on ur.RoleId= r.RoleId  "
                               + "  order by username";
            populateUserRole(rightOuterJoin, gvRightOuterJoin);

        }
        private string getApplicationName()
        {
            string info = Membership.ApplicationName + "<BR>";
            info += Membership.Provider + "<BR>";
            return info;

        }

        private void populateUserRole(string sqlCmd, GridView gvName)
        {
            string conString = CRUD.conStr; 
            SqlDataAdapter dad = new SqlDataAdapter(sqlCmd, conString);
            DataTable dtUserRoles = new DataTable();
            dad.Fill(dtUserRoles);
            gvName.DataSource = dtUserRoles;
            gvName.DataBind();
        }
        protected void btnUserRoleAssign_Click(object sender, EventArgs e)
        {
            string strX = " ";
            foreach (ListItem itemRole in cBLRoles.Items)
            {
                if (itemRole.Selected)
                {
                    foreach (ListItem itemUser in cBLUsers.Items)
                    {
                        if (itemUser.Selected && !Roles.IsUserInRole(itemUser.Text, itemRole.Text))
                            Roles.AddUserToRole(itemUser.Text, itemRole.Text);
                    }
                    strX += itemRole.Text;
                }

                else
                {
                    foreach (ListItem itemUser2 in cBLUsers.Items)
                    {
                        if (itemUser2.Selected && Roles.IsUserInRole(itemUser2.Text, itemRole.Text))
                            Roles.RemoveUserFromRole(itemUser2.Text, itemRole.Text);
                    }
                }
            }
            Response.Redirect(Request.Path);
            populateGvAllUsers();
        }
        
        protected void btnUpdateUser_Click(object sender, EventArgs e)
        {

        }

        protected void btnDeleteRoles_Click(object sender, EventArgs e)
        {
            foreach (ListItem itemRole in cBLRoles.Items)
            {
                if (itemRole.Selected)
                {
                    if (Roles.GetUsersInRole(itemRole.Text).Length == 0)
                        Roles.DeleteRole(itemRole.Text);
                    else
                        lblMsg.Text = "Cannot Delete Role when user is assigned it";
                }
            }
            populateGvAllUsers();
            populateCheckBoxListRolesUsers();
        }

        protected void btnDeleteUsers_Click(object sender, EventArgs e)
        {
            foreach (ListItem itemUser in cBLUsers.Items)
            {
                if (itemUser.Selected)
                {
                    Membership.DeleteUser(itemUser.Text);
                }
            }
            populateGvAllUsers();
            populateCheckBoxListRolesUsers();
        }
        protected void btnUnlinkUserRoles_Click(object sender, EventArgs e)
        {
            
            foreach (ListItem itemRole in cBLRoles.Items)
            {
                if (itemRole.Selected)
                {
                    foreach (ListItem itemUser in cBLUsers.Items)
                    {
                        if (itemUser.Selected)
                        {
                            if (Roles.IsUserInRole(itemUser.Text, itemRole.Text))
                            {
                                Roles.RemoveUserFromRole(itemUser.Text, itemRole.Text);
                                lblMsg.Text = "Unlinked Successfully";
                            }
                        }
                    }
                }
            }
            populateCheckBoxListRolesUsers();
            populateGvAllUsers();
        }

        private Guid GetUserId()
        {
            if (Membership.GetUser() is MembershipUser user)
            {
                return (Guid)user.ProviderUserKey;
            }
            return Guid.Empty;
        }

        public static void ExportGridToExcel(GridView myGv)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.Charset = "";
            string FileName = "ExportedReport_" + DateTime.Now + ".xls";
            StringWriter strwritter = new StringWriter();
            HtmlTextWriter htmltextwrtter = new HtmlTextWriter(strwritter);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);
            myGv.GridLines = GridLines.Both;
            myGv.HeaderStyle.Font.Bold = true;
            myGv.RenderControl(htmltextwrtter);
            HttpContext.Current.Response.Write(strwritter.ToString());
            HttpContext.Current.Response.End();
        }


    }
}