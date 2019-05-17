using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using etl.lib.util;
using System.Reflection;
using Microsoft.Office.Interop.Excel;

namespace etl.lib.loader
{
    public class ExcelLoader : AbstractLoader
    {
        //TODO - Create Excel Loader
        public override void load( System.Data.DataTable data)
        {
            // copy the template file to the output folder
            string targetFile = getTargetFile();
            string sheetName = SheetName;

            // copy the template file to a serialized filename or
            // create a new file with a serialized filename
            // open the target file using COM
            // write to the target sheet - check if it exists first.  If the target sheet does not exist, create it


            Microsoft.Office.Interop.Excel.Application xl_app = (Microsoft.Office.Interop.Excel.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");
                Excel.Workbook xl_workbook = null;

            // TODO - if the file exists, open the file
            // TODO - if the file does not exist, create a new file, add a sheet called <sheetName>
            /*
                xl_workbook = xl_app.ActiveWorkbook;
                Excel.Worksheet sheet = null;
                sheet = (Excel.Worksheet)xl_workbook.Worksheets.get_Item("Sheet1");
                sheet.Cells[1, 1] = "Name";
                
             */


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