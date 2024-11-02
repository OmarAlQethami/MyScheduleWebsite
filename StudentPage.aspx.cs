using MyScheduleWebsite.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MyScheduleWebsite
{
    public partial class StudentPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblGreeting.Text = "Hello " + User.Identity.Name + "!";
                Guid userId = GetUserId();
                lblCurrentLevel.Text = "Current Level: " + getCurrentLevel(userId);

                int universityId = getUniversityId(userId);
                int majorId = getMajorId(userId);
                BindSubjects(universityId, majorId);
            }
        }
        private Guid GetUserId()
        {
            if (Membership.GetUser() is MembershipUser user)
            {
                return (Guid)user.ProviderUserKey;
            }
            return Guid.Empty;
        }

        private int getCurrentLevel(Guid userId)
        {
            int currentLevel = 0;

            CRUD myCrud = new CRUD();
            string mySql = @"SELECT currentLevel FROM students WHERE UserId = @UserId";
            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@UserId", userId);

            SqlDataReader dr = myCrud.getDrPassSql(mySql, myPara);
            if (dr.Read())
            {
                currentLevel = dr.GetInt32(0);
            }
            dr.Close();
            return currentLevel;
        }

        private int getUniversityId(Guid userId)
        {
            int universityId = 0;

            CRUD myCrud = new CRUD();
            string mySql = @"SELECT universityId FROM students WHERE UserId = @UserId";
            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@UserId", userId);

            SqlDataReader dr = myCrud.getDrPassSql(mySql, myPara);
            if (dr.Read())
            {
                universityId = dr.GetInt32(0);
            }
            dr.Close();
            return universityId;
        }

        private int getMajorId(Guid userId)
        {
            int majorId = 0;

            CRUD myCrud = new CRUD();
            string mySql = @"SELECT majorId FROM students WHERE UserId = @UserId";
            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@UserId", userId);

            SqlDataReader dr = myCrud.getDrPassSql(mySql, myPara);
            if (dr.Read())
            {
                majorId = dr.GetInt32(0);
            }
            dr.Close();
            return majorId;
        }

        protected DataTable GetSubjectsByUniversityAndMajor(int universityId, int majorId)
        {
            CRUD myCrud = new CRUD();
            string mySql = @"SELECT * FROM subjects WHERE universityId = @universityId AND majorId = @majorId";

            Dictionary<string, object> myPara = new Dictionary<string, object>();
            myPara.Add("@universityId", universityId);
            myPara.Add("@majorId", majorId);

            DataTable dt = myCrud.getDTPassSqlDic(mySql, myPara);
            return dt;
        }


        private void BindSubjects(int universityId, int majorId)
        {
            CRUD crud = new CRUD();
            DataTable subjectsTable = GetSubjectsByUniversityAndMajor(universityId, majorId);

            // Group subjects by level
            var groupedSubjects = subjectsTable.AsEnumerable()
                .GroupBy(row => row.Field<int>("subjectLevel")); // Ensure this column exists in your table

            // Clear previous inner HTML to avoid duplication
            subjectsContainer.InnerHtml = string.Empty;

            // Iterate through each group of subjects
            foreach (var group in groupedSubjects)
            {
                // Create a new row for this level
                subjectsContainer.InnerHtml += "<div class='subjects-row'>";

                // Add subjects for this level to the row
                foreach (var subject in group)
                {
                    string subjectName = subject.Field<string>("subjectEnglishName");
                    // Add any other fields if needed, e.g., subjectCode
                    subjectsContainer.InnerHtml += $"<div class='subject'>{subjectName}</div>";
                }

                // Close the row div
                subjectsContainer.InnerHtml += "</div>"; // Close subjects-row
            }
        }


    }
}