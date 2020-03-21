using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using etl.lib.util;

namespace etl.lib.loader
{
    
    public class SqlServerLoader : AbstractLoader, ILoader
    {
        protected int commitLimit = 10000;
       

        protected SqlBulkCopy sqlBulkCopy = null;

        public SqlServerLoader()
        {

        }

        public SqlServerLoader(Arguments arg):base(arg)
        {

        }

        string Server
        {
            get
            {
                return arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
            }
        }

        string Database
        {
            get
            {
                return arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
            }
        }

        string Table
        {
            get
            {
                return arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
            }
        }

        public override void load( DataTable data)
        {
            connect(arguments);

            int rows = data.Rows.Count;

            System.Data.DataTable targetDataTable = createTargetTable(data);

            for (int r = 0, currentRows = 0; r < rows; r++, currentRows++)
            {
                DataRow srcRow = data.Rows[r];
                if (currentRows >= commitLimit)
                {
                    write(targetDataTable);
                    currentRows = 0;
                    Logger.info(string.Format("{0:P2}", ((double)r / (double)rows)));
                }

                fill(r, srcRow, targetDataTable);

            }
            write(targetDataTable);
            Logger.info("100.00 %");

            disconnect();
        }

        
        protected DataTable createTargetTable(DataTable data)
        {
            DataTable targetTable = data.Clone();
            
            for( int c = 0; c < data.Columns.Count; c++)
            {
                DataColumn dc = data.Columns[c];
                sqlBulkCopy.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);
            }
            return targetTable;
        }
      

        protected void fill(int row, DataRow srcRow, System.Data.DataTable targetDataTable)
        {
            DataRow dataRow = targetDataTable.NewRow();
            int cols = targetDataTable.Columns.Count;
            // System.Array values = row.Cells.Value;

            for (int c = 0; c < cols; c++)
            {
                DataColumn dc = targetDataTable.Columns[c];
                //Logger.info("filling column " + dc.ColumnName);
                try
                {
                    dataRow[dc] = srcRow[c];
                }
                catch (Exception x)
                {
                    Logger.error("Error processing column " + dc.ColumnName + " row " + row + ": " + x.Message);
                }
            }

            targetDataTable.Rows.Add(dataRow);
        }

        

        private string getConnectionString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Server=$server;DataBase=$database;Integrated Security=SSPI");
            sb.Replace("$server", Server);
            sb.Replace("$database", Database);

            return sb.ToString();
        }

        protected void connect(Arguments arg)
        {
            try
            {
                if (sqlBulkCopy != null) disconnect();

                sqlBulkCopy = new SqlBulkCopy(getConnectionString());
                
                sqlBulkCopy.DestinationTableName = Table;
            }
            catch (Exception x)
            {
                Logger.error("Cannot connect to target server. " + x.Message);
            }
        }

        protected void disconnect()
        {
            if (sqlBulkCopy != null)
            {
                sqlBulkCopy.Close();
                sqlBulkCopy = null;
            }
        }

        
    

        protected void write(System.Data.DataTable dataTable)
        {
            if (sqlBulkCopy == null) throw new Exception("Attempting to write when sqlBulkCopy is null.");

            try
            {
                sqlBulkCopy.WriteToServer(dataTable);
                dataTable.Rows.Clear();
            }
            catch (Exception x)
            {
                Logger.error("Error writing data: " + x.Message);
            }
        }
    }
}
