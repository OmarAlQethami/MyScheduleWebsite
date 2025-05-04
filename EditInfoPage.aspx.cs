using MyScheduleWebsite.App_Code;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.Security;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace MyScheduleWebsite
{
    public partial class EditInfoPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                Response.Redirect("~/Account/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                ShowPanelByRole();
            }
        }


        private void ShowPanelByRole()
        {
            if (Roles.IsUserInRole(User.Identity.Name, "admin"))
            {
                pnlAdmin.Visible = true;
                pnlStudent.Visible = false;
                pnlFaculty.Visible = false;
                pnlDepartmentHead.Visible = false;
                LoadAdminInfo();
            }
            else if (Roles.IsUserInRole(User.Identity.Name, "faculty"))
            {
                pnlAdmin.Visible = false;
                pnlStudent.Visible = false;
                pnlFaculty.Visible = true;
                pnlDepartmentHead.Visible = false;
                LoadFacultyInfo();
            }
            else if (Roles.IsUserInRole(User.Identity.Name, "student"))
            {
                pnlAdmin.Visible = false;
                pnlFaculty.Visible = false;
                pnlStudent.Visible = true;
                pnlDepartmentHead.Visible = false;
                LoadStudentInfo();
            }
            else if (Roles.IsUserInRole(User.Identity.Name, "departmentHead"))
            {
                pnlAdmin.Visible = false;
                pnlStudent.Visible = false;
                pnlFaculty.Visible = false;
                pnlDepartmentHead.Visible = true;
                LoadDepartmentHeadInfo();
            }
            else
            {
                lblOutput.Text = "You are not assigned to a valid role.";
                lblOutput.CssClass = "text-danger";
            }
        }

        private void LoadStudentInfo()
        {
            Guid userId = GetUserId();
            if (userId == Guid.Empty)
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            try
            {
                CRUD myCrud = new CRUD();
                string mySql = @"SELECT studentEnglishFirstName, studentEnglishLastName, 
                                        studentArabicFirstName, studentArabicLastName, 
                                        email, currentLevel, universityId, majorId
                                 FROM students 
                                 WHERE UserId = @UserId";

                Dictionary<string, object> myPara = new Dictionary<string, object>
                {
                    { "@UserId", userId }
                };

                using (SqlDataReader dr = myCrud.getDrPassSql(mySql, myPara))
                {
                    if (dr != null && dr.Read())
                    {
                        txtEnglishFirstName.Text = dr["studentEnglishFirstName"].ToString();
                        txtEnglishLastName.Text = dr["studentEnglishLastName"].ToString();
                        txtArabicFirstName.Text = dr["studentArabicFirstName"].ToString();
                        txtArabicLastName.Text = dr["studentArabicLastName"].ToString();
                        txtEmail.Text = dr["email"].ToString();

                        int universityId = Convert.ToInt32(dr["universityId"]);
                        int majorId = Convert.ToInt32(dr["majorId"]);
                        int currentLevel = Convert.ToInt32(dr["currentLevel"]);

                        PopulateCurrentLevels(universityId, majorId);
                        ddlCurrentLevel.SelectedValue = currentLevel.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                lblOutput.Text = "Error loading student info: " + ex.Message;
                lblOutput.CssClass = "text-danger";
            }
        }

        private void PopulateCurrentLevels(int universityId, int majorId)
        {
            ddlCurrentLevel.Items.Clear();
            ddlCurrentLevel.Items.Add(new ListItem("Choose your current level", ""));

            CRUD myCrud = new CRUD();
            string mySql = @"SELECT MAX(subjectLevel) AS MaxLevel 
                             FROM subjects 
                             WHERE universityId = @universityId AND majorId = @majorId";

            Dictionary<string, object> myPara = new Dictionary<string, object>()
            {
                { "@universityId", universityId },
                { "@majorId", majorId }
            };

            using (SqlDataReader dr = myCrud.getDrPassSql(mySql, myPara))
            {
                if (dr != null && dr.Read())
                {
                    int maxLevel = dr.IsDBNull(0) ? 0 : dr.GetInt32(0);
                    if (maxLevel > 0)
                    {
                        for (int i = 1; i <= maxLevel; i++)
                        {
                            ddlCurrentLevel.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        }
                    }
                }
            }
        }

        protected void btnSaveStudent_Click(object sender, EventArgs e)
        {
            Guid userId = GetUserId();
            if (userId == Guid.Empty)
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            string engFirstName = txtEnglishFirstName.Text.Trim();
            string engLastName = txtEnglishLastName.Text.Trim();
            string arabicFirstName = txtArabicFirstName.Text.Trim();
            string arabicLastName = txtArabicLastName.Text.Trim();
            string email = txtEmail.Text.Trim();
            int currentLevel = int.Parse(ddlCurrentLevel.SelectedValue);

            if (string.IsNullOrEmpty(engFirstName) || string.IsNullOrEmpty(engLastName) ||
                string.IsNullOrEmpty(arabicFirstName) || string.IsNullOrEmpty(arabicLastName) ||
                string.IsNullOrEmpty(email))
            {
                lblOutput.Text = "Fields cannot be empty.";
                lblOutput.CssClass = "text-danger";
                return;
            }

            try
            {
                CRUD myCrud = new CRUD();
                string mySql = @"UPDATE students 
                                 SET studentEnglishFirstName = @engFirstName, 
                                     studentEnglishLastName = @engLastName, 
                                     studentArabicFirstName = @arabicFirstName, 
                                     studentArabicLastName = @arabicLastName, 
                                     email = @email, 
                                     currentLevel = @currentLevel 
                                 WHERE UserId = @UserId";

                Dictionary<string, object> myPara = new Dictionary<string, object>
                {
                    { "@UserId", userId },
                    { "@engFirstName", engFirstName },
                    { "@engLastName", engLastName },
                    { "@arabicFirstName", arabicFirstName },
                    { "@arabicLastName", arabicLastName },
                    { "@email", email },
                    { "@currentLevel", currentLevel }
                };

                int rtn = myCrud.InsertUpdateDelete(mySql, myPara);

                if (!string.IsNullOrEmpty(txtNewPasswordStudent.Text))
                {
                    ChangePassword(txtCurrentPasswordStudent.Text, txtNewPasswordStudent.Text, txtConfirmPasswordStudent.Text);
                }

                lblOutput.Text = rtn >= 1 ? "Student information updated successfully!" : "No changes were made.";
                lblOutput.CssClass = rtn >= 1 ? "text-success" : "text-warning";
            }
            catch (Exception ex)
            {
                lblOutput.Text = "An error occurred: " + ex.Message;
                lblOutput.CssClass = "text-danger";
            }
        }

        protected void btnGoToProgress_Click(object sender, EventArgs e)
        {
            Response.Redirect("StudentProgressPage.aspx");

        }

        private void LoadAdminInfo()
        {
            var user = Membership.GetUser();
            if (user != null)
            {
                txtAdminEmail.Text = user.Email;
            }
        }

        protected void btnSaveAdmin_Click(object sender, EventArgs e)
        {
            var user = Membership.GetUser();
            if (user == null)
            {
                lblOutput.Text = "User not found.";
                lblOutput.CssClass = "text-danger";
                return;
            }

            user.Email = txtAdminEmail.Text.Trim();

            if (!string.IsNullOrEmpty(txtNewPassword.Text))
            {
                ChangePassword(txtCurrentPasswordAdmin.Text, txtNewPassword.Text, txtConfirmPassword.Text);
            }

            Membership.UpdateUser(user);
            lblOutput.Text = "Admin details updated successfully.";
            lblOutput.CssClass = "text-success";
        }

        private void LoadFacultyInfo()
        {
            var user = Membership.GetUser();
            if (user != null)
            {
                txtFacultyEmail.Text = user.Email;
            }
        }

        protected void btnSaveFaculty_Click(object sender, EventArgs e)
        {
            var user = Membership.GetUser();
            if (user == null)
            {
                lblOutput.Text = "User not found.";
                lblOutput.CssClass = "text-danger";
                return;
            }

            user.Email = txtFacultyEmail.Text.Trim();

            if (!string.IsNullOrEmpty(txtNewPasswordFaculty.Text))
            {
                ChangePassword(txtCurrentPasswordFaculty.Text, txtNewPasswordFaculty.Text, txtConfirmPasswordFaculty.Text);
            }

            Membership.UpdateUser(user);
            lblOutput.Text = "Faculty email updated successfully.";
            lblOutput.CssClass = "text-success";
        }

        protected void btnSaveDepartmentHead_Click(object sender, EventArgs e)
        {
            Guid userId = GetUserId();
            if (userId == Guid.Empty)
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            string email = txtDepartmentHeadEmail.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                lblOutput.Text = "Email cannot be empty.";
                lblOutput.CssClass = "text-danger";
                return;
            }

            try
            {
                CRUD myCrud = new CRUD();
                string mySql = @"UPDATE departmentHead SET email = @Email WHERE UserId = @UserId";

                Dictionary<string, object> myPara = new Dictionary<string, object>
                {
                    { "@Email", email },
                    { "@UserId", userId }
                };

                int result = myCrud.InsertUpdateDelete(mySql, myPara);
                lblOutput.Text = result >= 1 ? "Department head email updated successfully." : "No changes were made.";
                lblOutput.CssClass = result >= 1 ? "text-success" : "text-warning";
            }
            catch (Exception ex)
            {
                lblOutput.Text = "An error occurred: " + ex.Message;
                lblOutput.CssClass = "text-danger";
            }
        }

        private void LoadDepartmentHeadInfo()
        {
            Guid userId = GetUserId();
            if (userId == Guid.Empty)
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            try
            {
                CRUD myCrud = new CRUD();
                string mySql = @"SELECT email FROM departmentHead WHERE UserId = @UserId";

                Dictionary<string, object> myPara = new Dictionary<string, object>
                {
                    { "@UserId", userId }
                };

                using (SqlDataReader dr = myCrud.getDrPassSql(mySql, myPara))
                {
                    if (dr != null && dr.Read())
                    {
                        txtDepartmentHeadEmail.Text = dr["email"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                lblOutput.Text = "Error loading department head info: " + ex.Message;
                lblOutput.CssClass = "text-danger";
            }
        }

        private void ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var user = Membership.GetUser();
            if (user == null)
            {
                lblOutput.Text = "User not found.";
                lblOutput.CssClass = "text-danger";
                return;
            }

            if (string.IsNullOrEmpty(currentPassword))
            {
                lblOutput.Text = "Current password is required.";
                lblOutput.CssClass = "text-danger";
                return;
            }

            if (newPassword != confirmPassword)
            {
                lblOutput.Text = "New password and confirmation do not match.";
                lblOutput.CssClass = "text-danger";
                return;
            }

            if (!Membership.ValidateUser(user.UserName, currentPassword))
            {
                lblOutput.Text = "Current password is incorrect.";
                lblOutput.CssClass = "text-danger";
                return;
            }

            try
            {
                string tempPassword = user.ResetPassword();
                user.ChangePassword(tempPassword, newPassword);
                Membership.UpdateUser(user);

                lblOutput.Text = "Password changed successfully.";
                lblOutput.CssClass = "text-success";
            }
            catch (Exception ex)
            {
                lblOutput.Text = "Password change failed: " + ex.Message;
                lblOutput.CssClass = "text-danger";
            }
        }

        private Guid GetUserId()
        {
            var user = Membership.GetUser();
            if (user == null || user.ProviderUserKey == null)
            {
                return Guid.Empty;
            }
            return (Guid)user.ProviderUserKey;
        }
    }
}
