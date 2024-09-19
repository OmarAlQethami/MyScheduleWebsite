using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MyScheduleWebsite.App_Start
{
    public class ConfigLoader
    {
        public static void LoadEnvironmentVariables(string filePath)
        {
            foreach (var line in File.ReadAllLines(filePath))
            {
                if (!string.IsNullOrEmpty(line) && line.Contains("="))
                {
                    var keyValue = line.Split('=');
                    Environment.SetEnvironmentVariable(keyValue[0], keyValue[1]);
                }
            }
        }
    }
}