using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using etl.lib.util;

namespace etl.lib.transformer
{
    public class AbstractTransformer : ITransformer
    {
        public virtual DataTable transform( Arguments arg, DataTable data)
        {
            throw new NotImplementedException();
        }

        private object getValue(DataColumn dc, DataRow srcRow, int c)
        {
            object val = srcRow[c];

            if (val == null)
            {
                val = getDefaultValue(dc.DataType);
            }
            else
            {
                try
                {
                    val = Convert.ChangeType(val, dc.DataType);
                }
                catch
                {
                    val = getDefaultValue(dc.DataType);
                }
            }

            return val;
        }

        protected object getDefaultValue(Type dataType)
        {
            object val = System.DBNull.Value;

            if (dataType.Equals(typeof(string)))
            {
                val = string.Empty;
            }
            else if (dataType.Equals(typeof(int)))
            {
                val = (int)0;
            }
            else if (dataType.Equals(typeof(long)))
            {
                val = (long)0;
            }
            else if (dataType.Equals(typeof(double)))
            {
                val = (double)0.0;
            }
            else if (dataType.Equals(typeof(DateTime)))
            {
                val = new DateTime(1900, 1, 1);
            }
            return val;
        }
    }
}
