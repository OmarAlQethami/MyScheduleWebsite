using MyScheduleWebsite.App_Code;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MyScheduleWebsite.Account
{
    public partial class signUp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSignUp_Click(object sender, EventArgs e)
        {
            string newUser = txtUsername.Text.ToString();
            string newPassword = txtPassword.Text.ToString();
            string newEmail = txtEmail.Text.ToString();
            if (!Membership.ValidateUser(newUser, newPassword))
            {
                MembershipUser newUserObj = Membership.CreateUser(newUser, newPassword, newEmail);
                Guid userId = (Guid)newUserObj.ProviderUserKey;

                string strFName = txtFName.Text;
                string strLName = txtLName.Text;
                string strArFName = txtArFName.Text;
                string strArLName = txtArLName.Text;
                string strEmail = txtEmail.Text;
                string strStudentUniId = txtUniId.Text;
                string strUniversity = ddlUniversity.SelectedValue;
                string strMajor = ddlMajors.SelectedValue;
                string strLevel = ddlCurrentLevel.SelectedValue;

                int universityId = GetUniversityId(strUniversity);
                int majorId = GetMajorId(strMajor);

                CRUD myCrud = new CRUD();
                string mySql = @"INSERT INTO students (studentUniId, studentEnglishFirstName, studentEnglishLastName,
                           studentArabicFirstName, studentArabicLastName, email, currentLevel, universityId, majorId, UserId)
                           VALUES (@uniId, @fName, @lName, @arFName, @arLName, @email, @currentLevel, @universityId, @majorId, @UserId)";

                Dictionary<string, object> myPara = new Dictionary<string, object>();
                myPara.Add("@uniId", strStudentUniId);
                myPara.Add("@fName", strFName);
                myPara.Add("@lName", strLName);
                myPara.Add("@arFName", strArFName);
                myPara.Add("@arLName", strArLName);
                myPara.Add("@email", strEmail);
                myPara.Add("@currentLevel", strLevel);
                myPara.Add("@universityId", universityId);
                myPara.Add("@majorId", majorId);
                myPara.Add("@UserId", userId);
                int rtn = myCrud.InsertUpdateDelete(mySql, myPara);
                if (rtn >= 1)
                {
                    Roles.AddUserToRole(newUser, "student");
                    lblOutput.Text = "User Created Successfully. Please return to login page.";
                }
                else
                {
                    lblOutput.Text = "Signing up has failed. Please try again.";
                }
            }
            else
            {
                lblOutput.Text = "The Username you chose is unavailable. Please try with a different username.";
                return;
            }
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

            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@universityName", universityName);
            myPara.Add("@majorName", majorName);

            using (SqlDataReader dr = myCrud.getDrPassSql(mySql, myPara))
            {
                if (dr.Read())
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
        private int GetUniversityId(string universityName)
        {
            CRUD myCrud = new CRUD();
            string mySql = "SELECT universityId FROM universities WHERE universityEnglishName = @universityName";
            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@universityName", universityName);

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
            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@majorName", majorName);

            using (SqlDataReader dr = myCrud.getDrPassSql(mySql, myPara))
            {
                if (dr.Read())
                {
                    return dr.GetInt32(0);
                }
            }
            throw new Exception("Major ID not found.");
        }

    }
}