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
    public class Pythag32Transformer : AbstractTransformer, ITransformer
    {
        public Pythag32Transformer()
        {

        }

        public override DataTable transform(A DataTable data)
        {
            DataTable transformedData = new DataTable();

            transformedData.Columns.Add(new DataColumn("a", typeof(int)));
            transformedData.Columns.Add(new DataColumn("b", typeof(int)));
            transformedData.Columns.Add(new DataColumn("a2", typeof(int)));
            transformedData.Columns.Add(new DataColumn("b2", typeof(int)));
            transformedData.Columns.Add(new DataColumn("c2", typeof(int)));
            transformedData.Columns.Add(new DataColumn("c", typeof(float)));

            foreach (DataRow r in data.Rows)
            {
                int a = Convert.ToInt32((double) r["a"]);
                int b = Convert.ToInt32((double) r["b"]);
                int a2 = a * a;
                int b2 = b * b;
                int c2 = a2 + b2;
                float c = (float) Math.Sqrt((double)c2);

                DataRow xformRow = transformedData.NewRow();
                xformRow["a"] = a;
                xformRow["b"] = b;
                xformRow["a2"] = a2;
                xformRow["b2"] = b2;
                xformRow["c"] = c;
                xformRow["c2"] = c2;

                transformedData.Rows.Add(xformRow);
            }

            return transformedData;
        }


    }
}
