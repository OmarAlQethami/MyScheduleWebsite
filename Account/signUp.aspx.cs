using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;
using MyScheduleWebsite.App_Code;
using System.Web.UI;
using System.Web;

namespace MyScheduleWebsite.Account
{
    public partial class signUp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (ddlUniversity.Items.Count <= 1)
                {
                    PopulateUniversities();
                }
            }
        }

        private void PopulateUniversities()
        {
            CRUD myCrud = new CRUD();
            string mySql = "SELECT universityEnglishName FROM universities";
            Dictionary<string, object> myPara = new Dictionary<string, object>();

            using (SqlDataReader dr = myCrud.getDrPassSql(mySql, myPara))
            {
                ddlUniversity.Items.Clear();
                ddlUniversity.DataSource = dr;
                ddlUniversity.DataTextField = "universityEnglishName";
                ddlUniversity.DataValueField = "universityEnglishName";
                ddlUniversity.DataBind();
                ddlUniversity.Items.Insert(0, new ListItem("Choose a University", ""));
            }
        }

        protected void btnSignUp_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string newUser = txtUsername.Text.Trim();
                string newPassword = txtPassword.Text.Trim();
                string newEmail = txtEmail.Text.Trim();

                if (Membership.GetUser(newUser) == null)
                {
                    try
                    {
                        MembershipCreateStatus createStatus;
                        MembershipUser newUserObj = Membership.CreateUser(newUser, newPassword, newEmail, null, null, true, out createStatus);

                        if (createStatus == MembershipCreateStatus.Success)
                        {
                            Guid userId = (Guid)newUserObj.ProviderUserKey;

                            string strFName = txtFName.Text.Trim();
                            string strLName = txtLName.Text.Trim();
                            string strArFName = txtArFName.Text.Trim();
                            string strArLName = txtArLName.Text.Trim();
                            string strStudentUniId = txtUniId.Text.Trim();
                            int universityId = GetUniversityId(ddlUniversity.SelectedValue);
                            int majorId = GetMajorId(ddlMajors.SelectedValue);
                            string currentLevel = ddlCurrentLevel.SelectedValue;

                            string userFolder = Server.MapPath("~/Users/" + newUser);
                            CreateUserFolder(userFolder);

                            bool isStudentCreated = CreateStudent(strStudentUniId, strFName, strLName, strArFName, strArLName,
                                                                  newEmail, currentLevel, universityId, majorId, userId);

                            if (isStudentCreated)
                            {
                                Roles.AddUserToRole(newUser, "student");
                                FormsAuthentication.SetAuthCookie(newUser, false);
                                Session["Username"] = newUser;
                                Session["UserId"] = userId;
                                Session["Email"] = newEmail;
                                Session["FirstName"] = strFName;
                                Session["LastName"] = strLName;

                                Response.Redirect("~/StudentProgressPage.aspx");
                            }
                            else
                            {
                                Membership.DeleteUser(newUser, true);
                                DeleteUserFolder(userFolder);

                                lblOutput.Text = "Registration failed. Please try again.";
                                lblOutput.CssClass = "alert alert-danger";
                                lblOutput.Visible = true;
                            }
                        }
                        else
                        {
                            lblOutput.Text = $"User creation failed: {createStatus}";
                            lblOutput.CssClass = "alert alert-danger";
                            lblOutput.Visible = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        lblOutput.Text = "An error occurred: " + ex.Message;
                        lblOutput.CssClass = "alert alert-danger";
                        lblOutput.Visible = true;
                    }
                }
                else
                {
                    lblOutput.Text = "The username you chose is not available. Please try a different username.";
                    lblOutput.CssClass = "alert alert-danger";
                    lblOutput.Visible = true;
                }
            }
        }

        private void CreateUserFolder(string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void DeleteUserFolder(string folderPath)
        {
            try
            {
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private bool CreateStudent(string strStudentUniId, string strFName, string strLName, string strArFName,
                                  string strArLName, string newEmail, string currentLevel, int universityId,
                                  int majorId, Guid userId)
        {
            CRUD myCrud = new CRUD();
            string mySql = @"INSERT INTO students (studentUniId, studentEnglishFirstName, studentEnglishLastName,
                                       studentArabicFirstName, studentArabicLastName, email, currentLevel, universityId, majorId, UserId)
                                       VALUES (@uniId, @fName, @lName, @arFName, @arLName, @email, @currentLevel, @universityId, @majorId, @UserId)";

            Dictionary<string, object> myPara = new Dictionary<string, object>
                {
                    { "@uniId", strStudentUniId },
                    { "@fName", strFName },
                    { "@lName", strLName },
                    { "@arFName", strArFName },
                    { "@arLName", strArLName },
                    { "@email", newEmail },
                    { "@currentLevel", currentLevel },
                    { "@universityId", universityId },
                    { "@majorId", majorId },
                    { "@UserId", userId }
                };

            int rtn = myCrud.InsertUpdateDelete(mySql, myPara);
            return rtn >= 1;
        }

        private int GetUniversityId(string universityName)
        {
            CRUD myCrud = new CRUD();
            string mySql = "SELECT universityId FROM universities WHERE universityEnglishName = @universityName";
            Dictionary<string, object> myPara = new Dictionary<string, object>
                {
                    { "@universityName", universityName }
                };

            using (SqlDataReader dr = myCrud.getDrPassSql(mySql, myPara))
            {
                if (dr.Read())
                {
                    return dr.GetInt32(0);
                }
            }
            throw new Exception($"University ID not found for university: {universityName}.");
        }

        private int GetMajorId(string majorName)
        {
            CRUD myCrud = new CRUD();
            string mySql = "SELECT majorId FROM majors WHERE majorEnglishName = @majorName";
            Dictionary<string, object> myPara = new Dictionary<string, object>
                {
                    { "@majorName", majorName }
                };

            using (SqlDataReader dr = myCrud.getDrPassSql(mySql, myPara))
            {
                if (dr.Read())
                {
                    return dr.GetInt32(0);
                }
            }
            throw new Exception($"Major ID not found for major: {majorName}.");
        }

        protected void ddlUniversity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlUniversity.SelectedValue))
            {
                PopulateMajors(ddlUniversity.SelectedValue);
            }
            else
            {
                ddlMajors.Items.Clear();
                ddlMajors.Items.Insert(0, new ListItem("Choose a Major", ""));
                ddlCurrentLevel.Items.Clear();
                ddlCurrentLevel.Items.Insert(0, new ListItem("Choose your current level", ""));
            }
        }

        private void PopulateMajors(string universityName)
        {
            CRUD myCrud = new CRUD();
            string mySql = "SELECT majorEnglishName FROM majors WHERE universityId = (SELECT UniversityId from Universities WHERE universityEnglishName = @universityName)";
            Dictionary<string, object> myPara = new Dictionary<string, object>
                {
                     { "@universityName", universityName }
                 };

            using (SqlDataReader dr = myCrud.getDrPassSql(mySql, myPara))
            {
                ddlMajors.Items.Clear();
                ddlMajors.DataSource = dr;
                ddlMajors.DataTextField = "majorEnglishName";
                ddlMajors.DataValueField = "majorEnglishName";
                ddlMajors.DataBind();
                ddlMajors.Items.Insert(0, new ListItem("Choose a Major", ""));
            }
        }

        protected void ddlMajors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlMajors.SelectedValue))
            {
                PopulateCurrentLevel(ddlUniversity.SelectedValue, ddlMajors.SelectedValue);
            }
            else
            {
                ddlCurrentLevel.Items.Clear();
                ddlCurrentLevel.Items.Insert(0, new ListItem("Choose your current level", ""));
            }
        }

        private void PopulateCurrentLevel(string universityName, string majorName)
        {
            CRUD myCrud = new CRUD();
            string mySql = "SELECT MAX(subjectLevel) FROM subjects WHERE universityId =" +
                           "(SELECT universityId FROM universities WHERE universityEnglishName = @universityName) " +
                           "AND majorId = (SELECT majorId FROM majors WHERE majorEnglishName = @majorName)";

            Dictionary<string, object> myPara = new Dictionary<string, object>
                {
                    { "@universityName", universityName },
                    { "@majorName", majorName }
                };

            using (SqlDataReader dr = myCrud.getDrPassSql(mySql, myPara))
            {
                ddlCurrentLevel.Items.Clear();
                if (dr.Read())
                {
                    int maxLevel = dr.IsDBNull(0) ? 0 : dr.GetInt32(0);

                    for (int i = 1; i <= maxLevel; i++)
                    {
                        ddlCurrentLevel.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    }
                    ddlCurrentLevel.Items.Insert(0, new ListItem("Choose your current level", ""));
                }
                else
                {
                    ddlCurrentLevel.Items.Insert(0, new ListItem("No Levels Found", ""));
                }
            }
        }
    }
}
