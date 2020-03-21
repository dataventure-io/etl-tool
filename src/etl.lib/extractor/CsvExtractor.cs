using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using etl.lib.util;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace etl.lib.extractor
{
    public class CsvExtractor : AbstractExtractor, IExtractor
    {
        public CsvExtractor()
        { }

        public CsvExtractor(Arguments arg) : base(arg)
        { }

        public override DataTable extract()
        {
            DataTable dataTable = new DataTable();
            
            if (string.IsNullOrEmpty(SourceDirectory))
            {
                dataTable = processFile(dataTable, SourceFile);
            }
            else
            {
                dataTable = processDirectory(dataTable, SourceDirectory);
            }

            return dataTable;
        }

        DataTable processDirectory(DataTable dataTable, string sourceDirectory)
        {
            if (string.IsNullOrEmpty(sourceDirectory)) throw new EtlException(GetType(), "SourceDirectory must be specified.");
            if (string.IsNullOrWhiteSpace(sourceDirectory)) throw new EtlException(GetType(), "SourceDirectory must be specified.");
            if (!Directory.Exists(sourceDirectory)) throw new EtlException(GetType(), "SourceDirectory does not exist.");

            string[] files = Directory.GetFiles(sourceDirectory);
            foreach( string filename in files)
            {
                dataTable = processFile(dataTable, filename);
            }

            return dataTable;
        }

        DataTable processFile(DataTable dataTable, string filename)
        {
            if (string.IsNullOrEmpty(filename)) throw new EtlException(GetType(), "Filename cannot be null or empty");
            if (string.IsNullOrWhiteSpace(filename)) throw new EtlException(GetType(), "Filename must be specified.");
            if (!File.Exists(filename)) throw new EtlException(GetType(), filename + " does not exist.");

            string line;

            System.IO.StreamReader file =  new System.IO.StreamReader(filename);
            while ((line = file.ReadLine()) != null)
            {
                if (dataTable.Columns.Count == 0)
                {
                    string firstLine = line;
                    //string secondLine = file.ReadLine();

                    createDataColumns(dataTable, firstLine/*, secondLine*/);

                    //addRow(dataTable, secondLine);
                }
                else
                {
                    addRow(dataTable, line);
                }

            }

            file.Close();

            if (!string.IsNullOrEmpty(ProcessedFolder)) moveToProcessedFolder(filename, ProcessedFolder);
          
            return dataTable;
        }

        void createDataColumns( DataTable dataTable, string headers/*, string sampleText*/)
        {
            string[] colHeaders = parse(headers);
            //string[] sampleData = parse(sampleText);

            foreach( string s in colHeaders)
            {
                dataTable.Columns.Add(new DataColumn(s));
            }
        }

        void addRow( DataTable dataTable, string data)
        {
            string[] fields = parse(data);

            dataTable.Rows.Add(fields);
        }

        string[]  parse(string text)
        {
            AntlrInputStream inputStream = new AntlrInputStream(text);
            CSVLexer lexer = new CSVLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            CSVParser parser = new CSVParser(commonTokenStream);

            CSVParser.CsvFileContext csvCtx = parser.csvFile();
            CsvFileVisitor v = new CsvFileVisitor();
            return v.getValues(csvCtx);
        }

        void moveToProcessedFolder( string filename, string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            FileInfo fi = new FileInfo(filename);
            string basename = fi.Name + "." + fi.Extension;
            string destname = folder + "\\" + basename;
            File.Move(filename, destname);
        }
        public string SourceDirectory
        {
            get
            {
                return this.arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
            }
        }

        public string SourceFile
        {
            get
            {
                return this.arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
            }
        }

        public string Delimiter
        {
            get
            {
                return this.arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
            }
        }

        public string TextQualifier
        {
            get
            {
                return this.arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
            }
        }

        public string DateFormat
        {
            get
            {
                return this.arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
            }
        }

        public string ProcessedFolder
        {
            get
            {
                return this.arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
            }
        }

        public class Field
        {
            public Field()
            { }

            public string value = string.Empty;
        }

        public class Row
        {
            public Row()
            {

            }
            public List<Field> fields;
        }

        public class CsvFile
        {
            List<Row> rows;

            public CsvFile(List<Row> rows)
            {
                this.rows = rows;
            }
        }

        public class CsvFileVisitor : CSVBaseVisitor<CsvFile>
        {

           
            public string[] getValues(CSVParser.CsvFileContext context)
            {
                CSVParser.RowContext[] rowContexts = context.row();

                string[] result = null;

                if (rowContexts.Length > 0 )
                { 
                    CSVParser.FieldContext[] fca = rowContexts[0].field();
                    result = new string[fca.Length];
                    int i = 0;
                    foreach (CSVParser.FieldContext fc in fca)
                    {
                        result[i++] = fc.GetText();
                    }
                }

                return result;
            }
            #region scratch
            /*
           public override CsvFile VisitCsvFile([NotNull] CSVParser.CsvFileContext context)
           {

               List<Row> rows = new List<Row>();
               CSVParser.RowContext[] rowContexts = context.row();

               foreach (CSVParser.RowContext rc in rowContexts)
               {
                   Row r = new Row();

                   r.fields = getFields(rc.field());

                   rows.Add(r);
               }


               CsvFile csvFile = new CsvFile(rows);

               return csvFile;
           }
            public DataTable createDataTable(CSVParser.CsvFileContext context)
            {
                DataTable dataTable = new DataTable();

                CSVParser.RowContext[] rowContexts = context.row();

                createDataColumns(dataTable, rowContexts[0].field());

                return dataTable;
            }
            private List<Field> getFields(CSVParser.FieldContext[] fieldCtxArr)
            {
                List<Field> fields = new List<Field>();

                foreach (CSVParser.FieldContext fc in fieldCtxArr)
                {
                    Field f = new Field();
                    f.value = fc.GetText();
                    fields.Add(f);
                }

                return fields;
            }

            public void createDataColumns(DataTable dataTable, CSVParser.FieldContext[] fieldCtxArr)
            {
                foreach (CSVParser.FieldContext fc in fieldCtxArr)
                {
                    DataColumn dc = new DataColumn(fc.GetText());
                    dataTable.Columns.Add(dc);
                }
            }

            public void addRow(DataTable dataTable, CSVParser.FieldContext[] fieldCtxArr)
            {
                DataRow r = dataTable.NewRow();

                for (int i = 0; i < fieldCtxArr.Length; i++)
                {
                    CSVParser.FieldContext fc = fieldCtxArr[i];
                    r[i] = fc.GetText();
                }

                dataTable.Rows.Add(r);

            }
            */
            #endregion
        }
    }
}
