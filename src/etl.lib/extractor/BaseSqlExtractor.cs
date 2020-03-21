using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using etl.lib.util;

namespace etl.lib.extractor
{
    public class BaseSqlExtractor : AbstractExtractor
    {

        public BaseSqlExtractor()
        {

        }

        public BaseSqlExtractor(Arguments arg):base(arg)
        {

        }



        protected virtual string getConnectionString()
        {
            return "";
        }
        /*
         * Adapted from StackOverflow post
         * https://stackoverflow.com/questions/6073382/read-sql-table-into-c-sharp-datatable
         * 
         */
        public DataTable read<S, T>(string server, string database, string query) where S : IDbConnection, new()
                                           where T : IDbDataAdapter, IDisposable, new()
        {
            using (var conn = new S())
            {
                using (var da = new T())
                {
                    using (da.SelectCommand = conn.CreateCommand())
                    {
                        da.SelectCommand.CommandText = query;
                        da.SelectCommand.Connection.ConnectionString = getConnectionString();
                        DataSet ds = new DataSet(); //conn is opened by dataadapter
                        da.Fill(ds);
                        return ds.Tables[0];
                    }
                }
            }
        }
    }
}
