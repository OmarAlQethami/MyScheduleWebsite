using System;

using System.Data;

using System.IO;

using System.Web.UI;

using System.Web.UI.WebControls;



namespace MyScheduleWebsite

{

    public partial class OrderSuccessfulPage : System.Web.UI.Page

    {

        protected void Page_Load(object sender, EventArgs e)

        {

            if (!IsPostBack)

            {

                LoadStudentData();

                LoadScheduleData();

            }

        }



        private void LoadStudentData()

        {

            lblStudentName.Text = "Ahmed Al-Otaibi";

            lblUniID.Text = "123456789";

            lblMajor.Text = "Computer Science";

            lblTotalCredits.Text = "11";

        }



        private void LoadScheduleData()

        {

            DataTable dt = new DataTable();

            dt.Columns.Add("CourseCode");

            dt.Columns.Add("CourseName");

            dt.Columns.Add("SectionNumber");

            dt.Columns.Add("Credits");

            dt.Columns.Add("Schedule");

            dt.Columns.Add("Instructor");



            dt.Rows.Add("501472-3", "Computer Graphics", "244", "3", "Sun 8:00-11:00", "Dr.Al-Walid Al-Harbi");

            dt.Rows.Add("501427-3", "Programming Paradigms", "298", "3", "Mon 10:00-1:00", "Dr.Bader Al-Aoufi");

            dt.Rows.Add("501461-3", "Internet Technologies", "300", "3", "Tue 1:00-4:00", "Dr.Ibrahim Al-thomali");

            dt.Rows.Add("500321-2", "Professional Ethics", "286", "2", "Sun 11:00-1:00", "Dr.Sami Al-Suwat");



            gvSchedule.DataSource = dt;

            gvSchedule.DataBind();

        }



        protected void gvSchedule_RowDataBound(object sender, GridViewRowEventArgs e)

        {

            if (e.Row.RowType == DataControlRowType.DataRow)

            {

                TableCell sectionCell = e.Row.Cells[2];

                sectionCell.ToolTip = "Section Number: " + sectionCell.Text;

            }

        }



        protected void gvSchedule_Sorting(object sender, GridViewSortEventArgs e)

        {

            DataTable dt = GetScheduleData();

            dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);

            gvSchedule.DataSource = dt;

            gvSchedule.DataBind();

        }



        private string GetSortDirection(string column)

        {

            string sortDirection = "ASC";

            string sortExpression = ViewState["SortExpression"] as string;



            if (sortExpression != null && sortExpression == column)

            {

                string lastDirection = ViewState["SortDirection"] as string;

                if (lastDirection != null && lastDirection == "ASC")

                {

                    sortDirection = "DESC";

                }

            }



            ViewState["SortDirection"] = sortDirection;

            ViewState["SortExpression"] = column;



            return sortDirection;

        }



        private DataTable GetScheduleData()

        {

            DataTable dt = new DataTable();

            dt.Columns.Add("CourseCode");

            dt.Columns.Add("CourseName");

            dt.Columns.Add("SectionNumber");

            dt.Columns.Add("Credits");

            dt.Columns.Add("Schedule");

            dt.Columns.Add("Instructor");



            dt.Rows.Add("501472-3", "Computer Graphics", "244", "3", "Sun 8:00-11:00", "Dr.Al-Walid Al-Harbi");

            dt.Rows.Add("501427-3", "Programming Paradigms", "298", "3", "Mon 10:00-1:00", "Dr.Bader Al-Aoufi");

            dt.Rows.Add("501461-3", "Internet Technologies", "300", "3", "Tue 1:00-4:00", "Dr.Ibrahim Al-thomali");

            dt.Rows.Add("500321-2", "Professional Ethics", "286", "2", "Sun 11:00-1:00", "Dr.Sami Al-Suwat");



            return dt;

        }



        protected void btnExport_Click(object sender, EventArgs e)

        {



        }



        protected void btnCancel_Click(object sender, EventArgs e)

        {

            try

            {

                ClientScript.RegisterStartupScript(this.GetType(), "alert",
"alert('Schedule cancelled successfully!');", true);



                ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect",

                    "setTimeout(function(){ window.location.href = 'StudentDashboard.aspx'; }, 2000);", true);

            }

            catch (Exception ex)

            {

                ClientScript.RegisterStartupScript(this.GetType(), "alert",

                    $"alert('Error cancelling schedule: {ex.Message}');", true);

            }

        }

    }

}