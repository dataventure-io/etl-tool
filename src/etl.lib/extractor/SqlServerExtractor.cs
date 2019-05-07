using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using etl.lib.util;

namespace etl.lib.extractor
{
    public class SqlServerExtractor : BaseSqlExtractor, IExtractor 
    {
        public SqlServerExtractor():base()
        {

        }

        public string Server
        {
            get
            {
                return arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
            }
        }

        public string Database
        {
            get
            {
                return arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
            }
        }

        public string Query
        {
            get
            {
                return arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
            }
        }

        protected override string getConnectionString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Server=$server;DataBase=$database;Integrated Security=SSPI");
            sb.Replace("$server", Server);
            sb.Replace("$database", Database);

            return sb.ToString();
        }

        public override DataTable extract(Arguments arg)
        {
            base.extract(arg);

            DataTable dataTable = read<SqlConnection, SqlDataAdapter>(Server, Database, Query);

            return dataTable;
        }
    }
}
