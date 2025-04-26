using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MyScheduleWebsite.temp
{
    public partial class tempPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /*
            To use this page you need to install a package called EPPlus through NuGet Packages which allows Excel files manipulation.
            Install it then uncomment the line below.
        */

        protected void btnConvert_Click(object sender, EventArgs e)
        {
            try
            {
                if (!fuExcel.HasFile)
                {
                    lblMessage.Text = "Please select an Excel file";
                    return;
                }

                string uploadPath = Server.MapPath("~/temp/");
                string fileName = Path.Combine(uploadPath, "curriculum.xlsx");
                fuExcel.SaveAs(fileName);

                string outputPath = Server.MapPath("~/temp/sql_inserts.txt");
                // new ExcelToSqlConverter().GenerateInsertScripts(fileName, outputPath);
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"Error: {ex.Message}";
            }
        }

    }
}