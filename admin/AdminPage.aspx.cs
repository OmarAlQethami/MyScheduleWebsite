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
            }

            else
            {
                lblMsg.Text = "Unlinked Failed: User is not in the specified role";
                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('Unlinked Failed: User is not in the specified role')", true);
                return;
            }
        }

        //  protected void populateGvAllUsers()
        //  {
        //      
        //      string userCmd = "select username from dbo.aspnet_Users order by username";
        //    populateUserRole(userCmd, gvUsers);

        //      string roleCmd = "select rolename  from dbo.aspnet_Roles order by rolename";
        //     populateUserRole(roleCmd, gvRoles);

        //   string selectCommandNonANSI = @"select u.username ,r.rolename " +
        //                          "from dbo.aspnet_Users u, dbo.aspnet_Roles r  ,dbo.aspnet_UsersInRoles ur,dbo.aspnet_Applications  ap "
        //                        + "where u.UserId= ur.UserId  "
        //                        + "and ur.RoleId= r.RoleId "
        //                       + " and   applicationname ='" + appName + "'"
        //                          + "  order by username";
        //      populateUserRole(selectCommandNonANSI, gvNonAnsiInnerJoin);

        //       string innerJoin = @"select u.username ,r.rolename  "
        //                         + " from dbo.aspnet_Users u "
        //                         + " inner join dbo.aspnet_UsersInRoles ur on  u.UserId= ur.UserId   "
        //                         + " inner join   dbo.aspnet_Roles r   on ur.RoleId= r.RoleId  "
        //                         + " inner join dbo.aspnet_Applications  ap on u.applicationId= ap.applicationId"
        //                         + " where applicationname ='" + appName + "'"
        //                         + "  order by username";
        //      populateUserRole(innerJoin, gvInnerJoin);

        //      string leftOuterJoin = "select u.username ,r.rolename  "
        //                         + " from dbo.aspnet_Users u"
        //                         + " left outer join dbo.aspnet_UsersInRoles ur on  u.UserId= ur.UserId  "
        //                         + " left outer join   dbo.aspnet_Roles r   on ur.RoleId= r.RoleId  "
        //                         + " inner join dbo.aspnet_Applications  ap on u.applicationId= ap.applicationId"
        //                         + " where applicationname ='" + appName + "'"
        //                         + " order by username";
        //      populateUserRole(leftOuterJoin, gvLeftOuterJoin);

        //      string rightOuterJoin = "select distinct  u.username, r.rolename "
        //                         + " from dbo.aspnet_Applications ap inner join dbo.aspnet_Membership m on ap.applicationid=m.applicationid"
        //                         + " inner join dbo.aspnet_Users u on m.applicationid = u.applicationid "
        //                         + " right outer join dbo.aspnet_UsersInRoles ur on  u.UserId= ur.UserId   "
        //                         + " right outer join   dbo.aspnet_Roles r   on ur.RoleId= r.RoleId  "
        //                         + "  order by username";
        //      populateUserRole(rightOuterJoin, gvRightOuterJoin);

        //  }
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

        protected void btnSignUp_Click(object sender, EventArgs e)
        {
            try
            {
                
                string newUser = txtUsername1.Text.Trim();
                string newPassword = txtPassword1.Text;
                string newEmail = txtEmail1.Text.Trim().ToLower();

               
                if (string.IsNullOrEmpty(newUser) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(newEmail))
                {
                    ShowError("Please fill all required fields");
                    return;
                }

                
                if (!IsValidEmail(newEmail))
                {
                    ShowError("Please enter a valid email address");
                    return;
                }

                
                if (Membership.GetUser(newUser) != null)
                {
                    ShowError("This username is already registered to another user. Please choose a different username.");
                    return;
                }

                
                MembershipUser newUserObj = Membership.CreateUser(newUser, newPassword, newEmail);
                Guid userId = (Guid)newUserObj.ProviderUserKey;

                
                var facultyData = new
                {
                    fName = txtFName.Text.Trim(),
                    lName = txtLName.Text.Trim(),
                    arFName = txtArFName.Text.Trim(),
                    arLName = txtArLName.Text.Trim(),
                    email = newEmail,
                    universityId = GetUniversityId(ddlUniversity.SelectedValue),
                    majorId = GetMajorId(ddlMajors.SelectedValue),
                    UserId = userId
                };

                
                if (SaveFacultyData(facultyData))
                {
                    
                    if (!Roles.RoleExists("faculty"))
                        Roles.CreateRole("faculty");

                    Roles.AddUserToRole(newUser, "faculty");

                    ShowSuccess("Faculty account created successfully. Please send the credentials to the user");
                }
                else
                {
                   
                    Membership.DeleteUser(newUser);
                    ShowError("Registration failed. Please try again");
                }
            }
            catch (MembershipCreateUserException ex)
            {
                ShowError("Account creation error: " + GetMembershipError(ex.StatusCode));
            }
            catch (Exception ex)
            {
                ShowError("An unexpected error occurred: " + ex.Message);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool SaveFacultyData(dynamic facultyData)
        {
            try
            {
                CRUD myCrud = new CRUD();
                string mySql = @"INSERT INTO faculty 
                        (facultyEnglishFirstName, facultyEnglishLastName,
                         facultyArabicFirstName, facultyArabicLastName, 
                         email, universityId, majorId, UserId)
                        VALUES 
                        (@fName, @lName, @arFName, @arLName, 
                         @email, @universityId, @majorId, @UserId)";

                var myPara = new Dictionary<string, object>
        {
            {"@fName", facultyData.fName},
            {"@lName", facultyData.lName},
            {"@arFName", facultyData.arFName},
            {"@arLName", facultyData.arLName},
            {"@email", facultyData.email},
            {"@universityId", facultyData.universityId},
            {"@majorId", facultyData.majorId},
            {"@UserId", facultyData.UserId}
        };

                return myCrud.InsertUpdateDelete(mySql, myPara) >= 1;
            }
            catch
            {
                return false;
            }
        }

        private string GetMembershipError(MembershipCreateStatus status)
        {
            switch (status)
            {
                case MembershipCreateStatus.DuplicateUserName: return "Username already exists";
                case MembershipCreateStatus.DuplicateEmail: return "Email already registered";
                case MembershipCreateStatus.InvalidPassword: return "Invalid password";
                default: return "Unknown error";
            }
        }

        private void ShowError(string message)
        {
            lblSignUpOutput.Text = message;
            lblSignUpOutput.ForeColor = System.Drawing.Color.Red;
        }

        private void ShowSuccess(string message)
        {
            lblSignUpOutput.Text = message;
            lblSignUpOutput.ForeColor = System.Drawing.Color.Green;
        }
        protected void btnSignUp1_Click(object sender, EventArgs e)
        {
            string newUser = txtUserName2.Text.ToString();
            string newPassword = txtPass1.Text.ToString();
            string newEmail = txtEmail2.Text.ToString();

            if (!Membership.ValidateUser(newUser, newPassword))
            {
                try
                {
                    MembershipUser newUserObj = Membership.CreateUser(newUser, newPassword, newEmail);
                    Guid userId = (Guid)newUserObj.ProviderUserKey;

                    string strFName1 = txtFName1.Text;
                    string strLName1 = txtLName1.Text;
                    string strArFName1 = txtArFName1.Text;
                    string strArLName1 = txtArLName1.Text;
                    string strEmail2 = txtEmail2.Text;
                    string strUniversity1 = ddlUniversity1.SelectedValue;
                    string strMajor1 = ddlMajors1.SelectedValue;

                    int universityId = GetUniversityId(strUniversity1);
                    int majorId = GetMajorId(strMajor1);

                    CRUD myCrud = new CRUD();
                    string mySql = @"INSERT INTO departmentHead (departmentHeadEnglishFirstName, departmentHeadEnglishLastName,
                       departmentHeadArabicFirstName, departmentHeadArabicLastName, email, universityId, majorId, UserId)
                       VALUES (@fName, @lName, @arFName, @arLName, @email, @universityId, @majorId, @UserId)";

                    Dictionary<string, object> myPara = new Dictionary<string, object>();
                    myPara.Add("@fName", strFName1);
                    myPara.Add("@lName", strLName1);
                    myPara.Add("@arFName", strArFName1);
                    myPara.Add("@arLName", strArLName1);
                    myPara.Add("@email", strEmail2);
                    myPara.Add("@universityId", universityId);
                    myPara.Add("@majorId", majorId);
                    myPara.Add("@UserId", userId);

                    int rtn = myCrud.InsertUpdateDelete(mySql, myPara);
                    if (rtn >= 1)
                    {
                        if (!Roles.RoleExists("departmentHead"))
                        {
                            Roles.CreateRole("departmentHead");
                        }
                        Roles.AddUserToRole(newUser, "departmentHead");
                        lblSignUpOutput.Text = "Department Head account created successfully. Please send the login details to the user.";
                        lblSignUpOutput.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        Membership.DeleteUser(newUser); // حذف المستخدم إذا فشل حفظ البيانات
                        lblSignUpOutput.Text = "Failed to create department head account. Please try again.";
                        lblSignUpOutput.ForeColor = System.Drawing.Color.Red;
                    }
                }
                catch (MembershipCreateUserException ex)
                {
                    lblSignUpOutput.Text = "Error creating account: " + ex.Message;
                    lblSignUpOutput.ForeColor = System.Drawing.Color.Red;
                }
                catch (Exception)
                {
                    lblSignUpOutput.Text = "An error occurred. Please try again.";
                    lblSignUpOutput.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                lblSignUpOutput.Text = "This username is already taken. Please choose a different username.";
                lblSignUpOutput.ForeColor = System.Drawing.Color.Red;
            }
        }


        protected void ddlUniversity_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedUniversity = ddlUniversity.SelectedValue;
            PopulateMajors(selectedUniversity);
        }
        protected void ddlUniversity1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedUniversity = ddlUniversity1.SelectedValue;
            PopulateMajors1(selectedUniversity);
        }

        private void PopulateMajors(string universityName)
        {
            CRUD myCrud = new CRUD();
            string mySql = "SELECT majorEnglishName FROM majors WHERE universityId = (SELECT UniversityId from Universities WHERE universityEnglishName = @universityName)";

            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@universityName", universityName);

            using (SqlDataReader dr = myCrud.getDrPassSql(mySql, myPara))
            {
                ddlMajors.DataSource = dr;
                ddlMajors.DataTextField = "MajorEnglishName";
                ddlMajors.DataValueField = "MajorEnglishName";
                ddlMajors.DataBind();
            }
            ddlMajors.Items.Insert(0, new ListItem("Choose a Major", "0"));
        }
        private void PopulateMajors1(string universityName)
        {
            CRUD myCrud = new CRUD();
            string mySql = "SELECT majorEnglishName FROM majors WHERE universityId = (SELECT UniversityId from Universities WHERE universityEnglishName = @universityName)";

            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@universityName", universityName);

            using (SqlDataReader dr = myCrud.getDrPassSql(mySql, myPara))
            {
                ddlMajors1.DataSource = dr;
                ddlMajors1.DataTextField = "MajorEnglishName";
                ddlMajors1.DataValueField = "MajorEnglishName";
                ddlMajors1.DataBind();
            }
            ddlMajors1.Items.Insert(0, new ListItem("Choose a Major", "0"));
        }


        private int GetUniversityId(string universityName)
        {
            CRUD myCrud = new CRUD();
            string mySql = @"SELECT universityId FROM universities WHERE universityEnglishName = @universityName";
            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@universityName", universityName);

            using (SqlDataReader dr = myCrud.getDrPassSql(mySql, myPara))
            {
                if (dr.Read())
                {
                    return dr.GetInt32(0);
                }
            }
            return 0;
        }
        private int GetMajorId(string majorName)
        {
            CRUD myCrud = new CRUD();
            string mySql = "SELECT majorId FROM majors WHERE majorEnglishName = @majorName";
            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@majorName", majorName);

            using (SqlDataReader dr = myCrud.getDrPassSql(mySql, myPara))
            {
                if (dr.Read())
                {
                    return dr.GetInt32(0);
                }
            }
            return 0;
        }

        protected void btnAddMajors_Click(object sender, EventArgs e)
        {
            string universityName = txtUName.Text.Trim();
            string universityArName = txtUArName.Text.Trim();
            string majorsInput = txtMajors.Text.Trim();
            string majorsArInput = txtArMajors.Text.Trim();
            string totalCreditHours = txtTotal.Text.Trim();

            if (string.IsNullOrEmpty(universityName) || string.IsNullOrEmpty(universityArName))
            {
                lblMajorsOutput.Text = "Please enter both English and Arabic names for the university.";
                lblMajorsOutput.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (string.IsNullOrEmpty(majorsInput) || string.IsNullOrEmpty(majorsArInput))
            {
                lblMajorsOutput.Text = "Please enter at least one major in both languages.";
                lblMajorsOutput.ForeColor = System.Drawing.Color.Red;
                return;
            }

            string[] majors = majorsInput.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] majorsAr = majorsArInput.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (majors.Length != majorsAr.Length)
            {
                lblMajorsOutput.Text = "Mismatch between number of majors in English and Arabic.";
                lblMajorsOutput.ForeColor = System.Drawing.Color.Red;
                return;
            }

            try
            {
                // Get or create the university
                int universityId = GetUniversityId(universityName);
                if (universityId == 0)
                {
                    universityId = SaveUniversity(universityName, universityArName);
                }

                int addedMajors = 0;
                for (int i = 0; i < majors.Length; i++)
                {
                    string major = majors[i].Trim();
                    string majorAr = majorsAr[i].Trim();

                    int majorId = GetMajorId(major);
                    if (majorId == 0)
                    {
                        SaveMajor(universityId, majorAr, major, totalCreditHours);
                        addedMajors++;
                    }
                }

                if (addedMajors > 0)
                {
                    lblMajorsOutput.Text = "Majors have been added successfully.";
                    lblMajorsOutput.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblMajorsOutput.Text = "All entered majors already exist.";
                    lblMajorsOutput.ForeColor = System.Drawing.Color.OrangeRed;
                }

                txtMajors.Text = "";
                txtArMajors.Text = "";
                txtTotal.Text = "";
            }
            catch (Exception ex)
            {
                lblMajorsOutput.Text = "Error: " + ex.Message;
                lblMajorsOutput.ForeColor = System.Drawing.Color.Red;
            }
        }



        private int SaveUniversity(string universityName, string universityArName)
        {
            CRUD myCrud = new CRUD();
            string mySql = @"INSERT INTO universities (universityEnglishName, universityArabicName) 
                              VALUES (@universityEnglishName, @universityArabicName); 
                              SELECT SCOPE_IDENTITY();";

            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@UniversityEnglishName", universityName);
            myPara.Add("@universityArabicName", universityArName);

            int universityId = Convert.ToInt32(myCrud.InsertUpdateDelete(mySql, myPara));
            return universityId;
        }


        private int SaveMajor(int universityId, string majorsArInput, string majorsInput, string totalCreditHours)
        {
            CRUD myCrud = new CRUD();
            string mySql = @"INSERT INTO majors (majorEnglishName, majorArabicName, universityId, totalCreditHours) 
                         VALUES (@majorEnglishName, @majorArabicName, @universityId, @totalCreditHours)";

            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@majorEnglishName", majorsInput);
            myPara.Add("@majorArabicName", majorsArInput);
            myPara.Add("@universityId", universityId);
            myPara.Add("@totalCreditHours", totalCreditHours);

            int majorId = Convert.ToInt32(myCrud.InsertUpdateDelete(mySql, myPara));
            return majorId;

        }
    }
}