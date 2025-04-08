using MyScheduleWebsite.App_Code;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.Security;
using System.Data.SqlClient;

namespace MyScheduleWebsite
{
    public partial class EditInfoPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
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
                LoadAdminInfo();
            }
            else if (Roles.IsUserInRole(User.Identity.Name, "faculty"))
            {
                pnlAdmin.Visible = false;
                pnlStudent.Visible = false;
                pnlFaculty.Visible = true;
                LoadFacultyInfo();
            }
            else if (Roles.IsUserInRole(User.Identity.Name, "student"))
            {
                pnlAdmin.Visible = false;
                pnlFaculty.Visible = false;
                pnlStudent.Visible = true;
                LoadStudentInfo();
            }
            else
            {
                lblOutput.Text = "You are not assigned to a valid role.";
                lblOutput.CssClass = "text-danger";
            }
        }

        // === STUDENT SECTION ===
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
                                        email, currentLevel 
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
                        ddlCurrentLevel.SelectedValue = dr["currentLevel"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                lblOutput.Text = "Error loading student info: " + ex.Message;
                lblOutput.CssClass = "text-danger";
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
                lblOutput.Text = rtn >= 1 ? "Student information updated successfully!" : "No changes were made.";
                lblOutput.CssClass = rtn >= 1 ? "text-success" : "text-warning";
            }
            catch (Exception ex)
            {
                lblOutput.Text = "An error occurred: " + ex.Message;
                lblOutput.CssClass = "text-danger";
            }
        }

        // === ADMIN SECTION ===
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
                if (txtNewPassword.Text != txtConfirmPassword.Text)
                {
                    lblOutput.Text = "Passwords do not match!";
                    lblOutput.CssClass = "text-danger";
                    return;
                }

                try
                {
                    string tempPassword = user.ResetPassword();
                    user.ChangePassword(tempPassword, txtNewPassword.Text);
                }
                catch (Exception ex)
                {
                    lblOutput.Text = "Password change failed: " + ex.Message;
                    lblOutput.CssClass = "text-danger";
                    return;
                }
            }

            Membership.UpdateUser(user);
            lblOutput.Text = "Admin details updated successfully.";
            lblOutput.CssClass = "text-success";
        }

        // === FACULTY SECTION ===
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
            Membership.UpdateUser(user);

            lblOutput.Text = "Faculty email updated successfully.";
            lblOutput.CssClass = "text-success";
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
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
