using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using etl.lib.util;
using System.IO;
using System.Reflection;


namespace etl.lib.loader
{
    public class CsvLoader : AbstractFileLoader
    {
        public CsvLoader() : base()
        {

        }

        public CsvLoader(Arguments arg):base(arg)
        {

        }

        public override void load(DataTable data)
        {
            string targetFile = getTargetFile();

            TextWriter w = new StreamWriter(targetFile);

            setColumnHeaders(data, w);

            fill(data, w);

            w.Close();
            
        }

        private void fill(System.Data.DataTable data, TextWriter w)
        {
            for (int r = 0; r < data.Rows.Count; r++)
            {
                DataRow row = data.Rows[r];
                for (int c = 0; c < data.Columns.Count; c++)
                {
                    w.Write(row[c].ToString());
                    if (c < (data.Columns.Count - 1)) w.Write(Delimiter);
                }
                w.WriteLine();
            }
        }

        private void setColumnHeaders(System.Data.DataTable data, TextWriter w)
        {
            for (int c = 0; c < data.Columns.Count; c++)
            {
                w.Write(data.Columns[c].ColumnName);
                if (c < (data.Columns.Count - 1)) w.Write(Delimiter);
            }
            w.WriteLine();
        }
    }
}
