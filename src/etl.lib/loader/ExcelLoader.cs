using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using etl.lib.util;
using System.Reflection;
using Microsoft.Office.Interop.Excel;
using System.IO;

namespace etl.lib.loader
{
    public class ExcelLoader : AbstractLoader
    {
        //TODO - Create Excel Loader
        public override void load(System.Data.DataTable data)
        {
            string targetFile = getTargetFile();
            string sheetName = SheetName;

            var excelApp = new Microsoft.Office.Interop.Excel.Application();

            Microsoft.Office.Interop.Excel.Workbook workbook = getWorkbook(targetFile, excelApp); ;

            Microsoft.Office.Interop.Excel.Worksheet worksheet = getWorksheet(sheetName, workbook);

            setColumnHeaders(data, worksheet);

            fill(data, worksheet);
        }

        private  void fill(System.Data.DataTable data, Worksheet worksheet)
        {
            for (int r = 0; r < data.Rows.Count; r++)
            {
                DataRow row = data.Rows[r];
                for (int c = 0; c < data.Columns.Count; c++)
                {
                    Microsoft.Office.Interop.Excel.Range cell = worksheet.Cells[r + 2, c + 1];
                    cell.Value = row[c];
                }
            }
        }

        private  void setColumnHeaders(System.Data.DataTable data, Worksheet worksheet)
        {
            for (int c = 0; c < data.Columns.Count; c++)
            {
                Microsoft.Office.Interop.Excel.Range cell = worksheet.Cells[1, c];
                cell.Value = data.Columns[c].ColumnName;
            }
        }

        private  Worksheet getWorksheet(string sheetName, Workbook workbook)
        {
            Microsoft.Office.Interop.Excel.Worksheet worksheet = null;

            bool found = false;
            for (int i = 1; (i <= workbook.Worksheets.Count) && !found; i++)
            {
                worksheet = workbook.Worksheets[i];
                string name = worksheet.Name.ToLower();

                if (string.Equals(sheetName, name, StringComparison.CurrentCultureIgnoreCase))
                {
                    found = true;
                }
            }

            if (!found)
            {
                worksheet = workbook.Worksheets.Add();
                worksheet.Name = sheetName;
            }

            return worksheet;
        }

        private Workbook getWorkbook(string targetFile, Application excelApp)
        {
            Workbook workbook;
            if (File.Exists(targetFile))
            {
                workbook = excelApp.Workbooks.Open(targetFile);
            }
            else
            {
                workbook = excelApp.ActiveWorkbook;
            }

            return workbook;
        }

        string OutputFolder
        {
            //TODO - add macro support for Excel output folder location
            //TODO - create the output folder if it does not exist
            get
            {
                return arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
            }
        }

        string BaseOutputFileName
        {
            //TODO - serialize base ouput file name
            get
            {
                return arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
            }
        }

        string TemplateFileName
        {
            //TODO - copy from a starting Excel file if supplied
            get
            {
                return arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
            }
        }

        string SheetName
        {
            get
            {
                return arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
            }
        }

        protected string getTargetFile()
        {
            string targetFile = buildTargetFileName();
            if (!string.IsNullOrEmpty(TemplateFileName))
            {
                copyTemplate(targetFile);
            }

            return targetFile;
        }

        private string buildTargetFileName()
        {
            string templateFileName = TemplateFileName;
            string ext = "xlsx";
            if (!string.IsNullOrEmpty(templateFileName))
            {
                string[] parts = templateFileName.Split(new char[] { '.' });
                ext = parts[parts.Length - 1];
            }
            return OutputFolder + "\\" + BaseOutputFileName + "." + getSerial() + "." + ext;
        }

        protected void copyTemplate(string targetFile)
        {
            System.IO.File.Copy(TemplateFileName, targetFile);

        }

        protected string getSerial()
        {
            DateTime now = DateTime.Now;

            StringBuilder s = new StringBuilder();
            s.Append(now.Year.ToString());
            s.Append(now.Month.ToString().PadLeft(2, '0'));
            s.Append(now.Day.ToString().PadLeft(2, '0'));
            s.Append("_");
            s.Append(now.Hour.ToString().PadLeft(2, '0'));
            s.Append(now.Minute.ToString().PadLeft(2, '0'));
            s.Append(now.Second.ToString().PadLeft(2, '0'));
            s.Append("_");
            s.Append(now.Millisecond.ToString().PadLeft(3, '0'));

            return s.ToString();
        }
    }

}