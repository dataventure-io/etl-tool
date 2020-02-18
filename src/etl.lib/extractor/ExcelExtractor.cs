using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data.OleDb;
using etl.lib.util;

namespace etl.lib.extractor
{
    public class ExcelExtractor : AbstractExtractor, IExtractor
    {
        public ExcelExtractor()
        {

        }

        /*
         * Adapted from stackoverflow post
         * https://stackoverflow.com/questions/1206023/how-to-get-current-property-name-via-reflection
         */
        public string ExcelFile
        {
            get
            {
                return this.arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
            }
        }

        public string SheetName
        {
            get
            {
                return this.arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
            }
        }

        public override DataTable extract()
        {

            string fileName = ExcelFile;
            string sheetname = SheetName;

            var connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0; data source={0}; Extended Properties=\"Excel 12.0\";", fileName);

            var sql = "SELECT * FROM [" + sheetname + "$]";

            var adapter = new OleDbDataAdapter(sql, connectionString);
            var ds = new DataSet();

            adapter.Fill(ds, sheetname);

            DataTable data = ds.Tables[sheetname];

            return data;
        }
    }
}
