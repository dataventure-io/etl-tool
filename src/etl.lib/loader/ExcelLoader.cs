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
    public class ExcelLoader : AbstractFileLoader
    {
        public ExcelLoader()
        {

        }
        //TODO - Create Excel Loader
        public override void load(System.Data.DataTable data)
        {
            string targetFile = getTargetFile();
            string sheetName = SheetName;

            var excelApp = new Microsoft.Office.Interop.Excel.Application();

            Microsoft.Office.Interop.Excel.Workbook workbook = getWorkbook(targetFile, excelApp); 

            Microsoft.Office.Interop.Excel.Worksheet worksheet = getWorksheet(sheetName, workbook);

            setColumnHeaders(data, worksheet);

            fill(data, worksheet);

            workbook.SaveAs(targetFile);
            workbook.Close();
            excelApp.Quit();
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
                Microsoft.Office.Interop.Excel.Range cell = worksheet.Cells[1, c+1];
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
                if (workbook == null)
                {
                    workbook = excelApp.Workbooks.Add();
                }
            }

            return workbook;
        }

        string SheetName
        {
            get
            {
                return arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
            }
        }

       

       
    }

}