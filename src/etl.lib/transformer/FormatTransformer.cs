using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using etl.lib.util;

namespace etl.lib.transformer
{
    public class FormatTransformer : AbstractTransformer, ITransformer
    {
        class ColDef
        {
            public string fromName;
            public string fromType;
            public string toName;
            public string toType;

            private string getFormatPart(string[] parts, int i)
            {
                string result = string.Empty;
                if (parts.Length > i) result = parts[i];
                return result;
            }
        }

        Dictionary<string, ColDef> columnMap = new Dictionary<string, ColDef>();

        public override DataTable transform( DataTable data)
        {
            DataTable result = new DataTable();

            loadFormatFile();

            return result;
        }

        protected void loadFormatFile()
        {
            string line;

            System.IO.StreamReader file = new System.IO.StreamReader(FormatFile);
            while ((line = file.ReadLine()) != null)
            {
                ColDef colDef = new ColDef();
                string[] parts = line.Split(new char[] { ':', '=' }); ;
                colDef.fromName = getFormatPart(parts, i++); ;
                colDef.fromType;
                colDef.toName;
                colDef.toType;

                parts = line.Split(new char[] { ':', '=' });

                int i = 0;
                fromName = 

                fromType = parts[1];

                System.Console.WriteLine(line);
            }

            file.Close();
        }

    

        string FormatFile
        {
            get
            {
                return arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
            }
        }
    }



    
}
