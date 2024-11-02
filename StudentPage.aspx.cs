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

            var groupedSubjects = subjectsTable.AsEnumerable()
                .GroupBy(row => row.Field<int>("subjectLevel"));

            subjectsContainer.InnerHtml = string.Empty;

            foreach (var group in groupedSubjects)
            {
                subjectsContainer.InnerHtml += "<div class='subjects-row'>";

                foreach (var subject in group)
                {
                    string subjectCode = subject.Field<string>("subjectCode");
                    string subjectName = subject.Field<string>("subjectEnglishName");
                    subjectsContainer.InnerHtml += $"<div class='subject'><asp:Button ID={subjectCode} runat=\"server\" CssClass=\"subject\" />{subjectName}</asp:Button></div>";
                }

                subjectsContainer.InnerHtml += "</div>";
            }
        }
    }
}