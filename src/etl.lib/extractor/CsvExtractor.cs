using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using FileHelpers;
using FileHelpers.Dynamic;
using etl.lib.util;
using FileHelpers.Detection;

namespace etl.lib.extractor
{
    public class CsvExtractor : AbstractExtractor, IExtractor
    {
        public override DataTable extract()
        {

            DataTable dataTable = null;
            
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



            var detector = new FileHelpers.Detection.SmartFormatDetector();
            RecordFormatInfo format = findBestFormat(detector.DetectFileFormat(filename));

            Logger.info("Format Detected, confidence:" + format.Confidence + "%");
            var delimited = format.ClassBuilderAsDelimited;

            Logger.info("    Delimiter:" + delimited.Delimiter);
            Logger.info("    Fields:");

            foreach (var field in delimited.Fields)
            {
                Logger.info("        " + field.FieldName + ": " + field.FieldType);
            }

            FileHelperEngine engine = new FileHelperEngine(delimited.CreateRecordClass());

            DataTable fileDataTable = engine.ReadFileAsDT(filename);

            if (dataTable == null)
            {
                dataTable = fileDataTable;
            }
            else
            {
                dataTable.Merge(fileDataTable);
            }

            return dataTable;
        }

        private  RecordFormatInfo findBestFormat(RecordFormatInfo[] formats)
        {
            RecordFormatInfo bestFormat = null;

            foreach (RecordFormatInfo format in formats)
            {
                if (bestFormat == null)
                {
                    bestFormat = format;
                }
                else
                {
                    if (format.Confidence > bestFormat.Confidence)
                    {
                        bestFormat = format;
                    }
                }
            }

            return bestFormat;
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
    }
}
