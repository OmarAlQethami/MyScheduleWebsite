//using OfficeOpenXml;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;


        /*
            To use this page you need to install a package called EPPlus through NuGet Packages which allows Excel files manipulation.
            Install it then uncomment this file and remove this comment.
         */


//public class ExcelToSqlConverter
//{
//    public void GenerateInsertScripts(string excelFilePath, string outputPath)
//    {
//        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
//        var allInserts = new List<string>();

//        using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
//        {
//            var worksheet = package.Workbook.Worksheets[0];
//            int rowCount = worksheet.Dimension.End.Row;

//            int startDataRow = 0;
//            for (int row = startDataRow; row <= rowCount; row++)
//            {
//                try
//                {
//                    string sectionNumberRaw = GetCellValue(worksheet, row, 19);
//                    if (!IsValidSectionNumber(sectionNumberRaw)) continue;

//                    int sectionNumber = ParseNumeric(sectionNumberRaw, row, "section number");
//                    int capacity = ParseNumeric(GetCellValue(worksheet, row, 12), row, "capacity");
//                    int day = ParseNumeric(GetCellValue(worksheet, row, 9), row, "day");

//                    string subjectCode = EscapeSqlString(GetCellValue(worksheet, row, 18).Trim());
//                    string instructorName = EscapeSqlString(GetCellValue(worksheet, row, 5).Trim());
//                    string location = EscapeSqlString(GetCellValue(worksheet, row, 6).Trim());

//                    string startTime = ParseTime(GetCellValue(worksheet, row, 8), row);
//                    string endTime = ParseTime(GetCellValue(worksheet, row, 7), row);

//                    allInserts.Add($@"
//INSERT INTO [dbo].[sections] (
//    [curriculumId], 
//    [sectionNumber],
//    [subjectCode],
//    [capacity],
//    [instuctorArabicName]
//) VALUES (
//    2,
//    {sectionNumber},
//    N'{subjectCode}',
//    {capacity},
//    N'{instructorName}'
//);");

//                    allInserts.Add($@"
//INSERT INTO [dbo].[sectionDetails] (
//    [sectionId],
//    [day],
//    [startTime],
//    [endTime],
//    [location]
//) VALUES (
//    (SELECT sectionId FROM [dbo].[sections] 
//     WHERE curriculumId = 2 AND sectionNumber = {sectionNumber}),
//    {day},
//    '{startTime}',
//    '{endTime}',
//    N'{location}'
//);");
//                }
//                catch (Exception ex)
//                {
//                    allInserts.Add($"-- ERROR IN ROW {row}: {ex.Message.Replace("\r\n", " ")}");
//                }
//            }
//        }
//        File.WriteAllLines(outputPath, allInserts, Encoding.UTF8);
//    }

//    private string GetCellValue(ExcelWorksheet worksheet, int row, int col)
//    {
//        return worksheet.Cells[row, col].Text.Trim();
//    }

//    private bool IsValidSectionNumber(string input)
//    {
//        return !string.IsNullOrWhiteSpace(input);
//    }

//    private int ParseNumeric(string input, int row, string fieldName)
//    {
//        try
//        {
//            string clean = new string(input?.Where(char.IsDigit).ToArray());

//            if (string.IsNullOrEmpty(clean))
//                throw new ArgumentException($"Empty {fieldName}");

//            return int.Parse(clean);
//        }
//        catch (Exception ex)
//        {
//            throw new FormatException($"{fieldName} '{input}': {ex.Message}");
//        }
//    }

//    private string ParseTime(string timeStr, int row)
//    {
//        try
//        {
//            timeStr = timeStr.Trim();

//            if (!Regex.IsMatch(timeStr, @"^(AM|PM)\s+\d{1,2}:\d{2}$", RegexOptions.IgnoreCase))
//                throw new FormatException("Invalid time format");

//            string[] parts = timeStr.Split(' ');
//            string[] timeParts = parts[1].Split(':');

//            int hours = int.Parse(timeParts[0]);
//            int minutes = int.Parse(timeParts[1]);

//            if (parts[0].Equals("PM", StringComparison.OrdinalIgnoreCase) && hours < 12)
//                hours += 12;

//            if (parts[0].Equals("AM", StringComparison.OrdinalIgnoreCase) && hours == 12)
//                hours = 0;

//            return $"{hours:D2}:{minutes:D2}";
//        }
//        catch (Exception ex)
//        {
//            throw new FormatException($"Invalid time '{timeStr}': {ex.Message}");
//        }
//    }

//    private string EscapeSqlString(string input)
//    {
//        return input?.Replace("'", "''") ?? string.Empty;
//    }
//}