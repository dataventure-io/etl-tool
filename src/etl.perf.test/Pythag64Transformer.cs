using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using etl.lib.util;
using etl.lib.transformer;

namespace etl.yahoo.fin
{
    public class Pythag64Transformer : AbstractTransformer, ITransformer
    {
        public Pythag64Transformer()
        {

        }

        public override DataTable transform( DataTable data)
        {
            DataTable transformedData = new DataTable();

            transformedData.Columns.Add(new DataColumn("a", typeof(long)));
            transformedData.Columns.Add(new DataColumn("b", typeof(long)));
            transformedData.Columns.Add(new DataColumn("a2", typeof(long)));
            transformedData.Columns.Add(new DataColumn("b2", typeof(long)));
            transformedData.Columns.Add(new DataColumn("c2", typeof(long)));
            transformedData.Columns.Add(new DataColumn("c", typeof(decimal)));

            foreach (DataRow r in data.Rows)
            {
                long a = Convert.ToInt64((double) r["A"]);
                long b = Convert.ToInt64((double) r["B"]);
                long a2 = a * a;
                long b2 = b * b;
                long c2 = a2 + b2;
                decimal c = (decimal) Math.Sqrt((double)c2);

                DataRow xformRow = transformedData.NewRow();
                xformRow["a"] = a;
                xformRow["b"] = b;
                xformRow["a2"] = a2;
                xformRow["b2"] = b2;
                xformRow["c2"] = c2;
                xformRow["c"] = c;
                

                transformedData.Rows.Add(xformRow);
            }

            return transformedData;
        }


    }
}
