using MyScheduleWebsite.App_Start;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace MyScheduleWebsite
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ConfigLoader.LoadEnvironmentVariables(Server.MapPath("~/env.config"));

            InitializeConfiguration();

        }

        private void InitializeConfiguration()
        {
            string dataSource = Environment.GetEnvironmentVariable("DB_DATA_SOURCE");
            string dbName = Environment.GetEnvironmentVariable("DB_NAME");

            string newConnectionString = $"Data Source={dataSource};Initial Catalog={dbName};Integrated Security=True";

            var config = WebConfigurationManager.OpenWebConfiguration("~");
            var connectionStringsSection = config.ConnectionStrings;
            var currentConnectionString = connectionStringsSection.ConnectionStrings["MyScheduleWebsiteConStr"]?.ConnectionString;

            if (currentConnectionString != newConnectionString)
            {
                connectionStringsSection.ConnectionStrings["MyScheduleWebsiteConStr"].ConnectionString = newConnectionString;
                config.Save();
            }
        }
    }
}