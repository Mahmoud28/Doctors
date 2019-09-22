using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;


namespace Seagull.Core.Helper.Export
{
    public static class ExportHelper
    {
        #region Helpers
        public static string GenerateRandomDigitCode(int length)
        {
            var random = new Random();
            string str = string.Empty;
            for (int i = 0; i < length; i++)
                str = String.Concat(str, random.Next(10).ToString());
            return str;
        }
        #endregion
        #region Methods

        public static void ExportToExcell(System.Data.DataTable query, string ReportName, List<string> ColumnName, HttpResponse Response)
        {
            
        }

        //public static void ExportToExcellWithMultipleSheets(string ReportName, List<MultipleSheetExport> Sheets, string Report = "")
        //{
            
        //}

        public static void ExportToPDF(List<object> query, string ReportName, List<string> ColumnName)
        {
            
        }

        public static byte[] ExportModel(DataTable query, List<ExportColumns> ColumnName, string workSheetName)
        {           
            byte[] result;

            using (var package = new ExcelPackage())
            {
                // add a new worksheet to the empty workbook

                var worksheet = package.Workbook.Worksheets.Add(workSheetName); //Worksheet name

                using (var cells = worksheet.Cells[1, 1, 1, 5]) //(1,1) (1,5)
                {
                    cells.Style.Font.Bold = true;
                }

                //First add the headers
                for (var i = 0; i < ColumnName.Count(); i++)
                {
                    worksheet.Cells[1, i + 1].Value = ColumnName[i].ColumnName;
                }

                for (var i = 1; i <= query.Rows.Count; i++)
                {
                    int count = 1;
                    // For each colum Id in ColumnName List 
                    foreach (ExportColumns columnId in ColumnName)
                    {
                        // For each row, print the values of each column.
                        DataRow row = query.Rows[i - 1];
                        worksheet.Cells[i + 1, count].Value = row[columnId.ColumnId].ToString();
                        count++;
                    }
                }
                result = package.GetAsByteArray();
            }
            return result;
        }
        #endregion

        #region Custom Classes

        public class ExportColumns
        {
            public string ColumnName { get; set; }
            public string ColumnId { get; set; }
        }
        #endregion
    }
}