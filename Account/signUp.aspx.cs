using MyScheduleWebsite.App_Code;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace MyScheduleWebsite.Account
{
    public partial class signUp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
        }

        protected void btnSignUp_Click(object sender, EventArgs e)
        {
            string confirmPassword = txtConfirmPassword.Text.Trim();
            string newUser = txtUsername.Text.Trim();
            string newPassword = txtPassword.Text.Trim();
            string newEmail = txtEmail.Text.Trim();
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(newUser))
            {
                lblUsernameError.Text = "Please enter a username.";
            }
            else
            {
                lblUsernameError.Text = "";
            }

            string strFName = txtFName.Text.Trim();
            string strLName = txtLName.Text.Trim();

            if (string.IsNullOrEmpty(strFName) || !IsEnglish(strFName))
            {
                lblFNameError.Text = "Please enter the first name in English.";
            }
            else
            {
                lblFNameError.Text = "";
            }

            if (string.IsNullOrEmpty(strLName) || !IsEnglish(strLName))
            {
                lblLNameError.Text = "Please enter the last name in English.";
            }
            else
            {
                lblLNameError.Text = "";
            }

            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
            {
                lblPasswordError.Text = "Password must be at least 6 characters.";
            }
            else
            {
                lblPasswordError.Text = "";
            }

            if (newPassword != confirmPassword)
            {
                lblConfirmPasswordError.Text = "Password and confirmation do not match.";
            }
            else
            {
                lblConfirmPasswordError.Text = "";
            }

            string strMajor = ddlMajors.SelectedValue;
            if (string.IsNullOrEmpty(strMajor) || strMajor == "0")
            {
                lblMajorError.Text = "Please select a major.";
            }
            else
            {
                lblMajorError.Text = "";
            }

            string strStudentUniId = txtUniId.Text.Trim();
            if (string.IsNullOrEmpty(strStudentUniId))
            {
                lblUniIdError.Text = "Please enter the university ID.";
            }
            else
            {
                lblUniIdError.Text = "";
            }

            string strUniversity = ddlUniversity.SelectedValue;
            if (string.IsNullOrEmpty(strUniversity))
            {
                lblUniversityError.Text = "Please select a university.";
            }
            else
            {
                lblUniversityError.Text = "";
            }

            if (lblFNameError.Text != "" || lblLNameError.Text != "" || lblPasswordError.Text != "" ||
                lblConfirmPasswordError.Text != "" || lblMajorError.Text != "" || lblUniIdError.Text != "" ||
                lblUniversityError.Text != "" || lblUsernameError.Text != "")
            {
                return;
            }

            if (Membership.GetUser(newUser) == null)
            {
                MembershipUser newUserObj = Membership.CreateUser(newUser, newPassword, newEmail);
                Guid userId = (Guid)newUserObj.ProviderUserKey;

                string strArFName = txtArFName.Text.Trim();
                string strArLName = txtArLName.Text.Trim();
                int universityId = GetUniversityId(strUniversity);
                int majorId = GetMajorId(strMajor);

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
                    { "@currentLevel", ddlCurrentLevel.SelectedValue },
                    { "@universityId", universityId },
                    { "@majorId", majorId },
                    { "@UserId", userId }
                };

                int rtn = myCrud.InsertUpdateDelete(mySql, myPara);

                if (rtn >= 1)
                {
                    Roles.AddUserToRole(newUser, "student");
                    lblOutput.Text = "User created successfully. You will be redirected to the student progress page...";
                    Response.Redirect("~/StudentProgressPage.aspx");
                }
                else
                {
                    Membership.DeleteUser(newUser, true);
                    lblOutput.Text = "Registration failed. Please try again.";
                    lblOutput.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                lblOutput.Text = "The username you chose is not available. Please try a different username.";
                lblOutput.ForeColor = System.Drawing.Color.Red;
                lblOutput.Visible = true;
            }
        }

        private bool IsEnglish(string input)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(input, @"^[a-zA-Z]+$");
        }

        protected void ddlUniversity_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedUniversity = ddlUniversity.SelectedValue;
            PopulateMajors(selectedUniversity);
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
            }
        }

        protected void ddlMajors_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedUniversity = ddlUniversity.SelectedValue;
            string selectedMajor = ddlMajors.SelectedValue;
            PopulateCurrentLevel(selectedUniversity, selectedMajor);
        }

        protected void PopulateCurrentLevel(string universityName, string majorName)
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
                if (dr.Read())
                {
                    int maxLevel = dr.IsDBNull(0) ? 0 : dr.GetInt32(0);
                    if (maxLevel > 0)
                    {
                        ddlCurrentLevel.Items.Clear();
                        for (int i = 1; i <= maxLevel; i++)
                        {
                            ddlCurrentLevel.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        }
                    }
                }
            }
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
            throw new Exception("University ID not found.");
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
            throw new Exception("Major ID not found: " + majorName);
        }
    }
}