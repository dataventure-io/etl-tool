using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace etl.lib.loader
{
    public class AbstractFileLoader : AbstractLoader
    {
        protected string _defaultDelim = ",";
        protected string _defaultOutputDir = ".";
        

        protected string OutputFolder
        {
            //TODO - create the output folder if it does not exist
            get
            {
                string outputDir =  arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
                if (string.IsNullOrEmpty(outputDir)) outputDir = _defaultOutputDir;
                return outputDir;
            }
        }

        protected string Delimiter
        {
            get
            {
                string delim =  arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
                if (string.IsNullOrEmpty(delim)) delim = _defaultDelim;
                return delim;
            }
        }

        protected string BaseOutputFileName
        {
            get
            {
                string baseName = arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
                if (string.IsNullOrEmpty(baseName)) baseName = etl.lib.util.Config.Current.BaseName;
                return baseName;
            }
        }
        
        protected string TemplateFileName
        {
            //TODO - test starting from template Excel file if supplied
            get
            {
                return arguments.getValue(GetType(), MethodBase.GetCurrentMethod().Name);
            }
        }

        protected string getTargetFile()
        {
            string targetFile = buildTargetFileName();
            if (!string.IsNullOrEmpty(TemplateFileName))
            {
                copyTemplate(targetFile);
            }

            return targetFile;
        }

        protected string buildTargetFileName()
        {
            string templateFileName = TemplateFileName;
            string ext = "xlsx";
            if (!string.IsNullOrEmpty(templateFileName))
            {
                string[] parts = templateFileName.Split(new char[] { '.' });
                ext = parts[parts.Length - 1];
            }
            return OutputFolder + "\\" + BaseOutputFileName + "." + getSerial() + "." + ext;
        }

        protected void copyTemplate(string targetFile)
        {
            System.IO.File.Copy(TemplateFileName, targetFile);

        }

        protected string getSerial()
        {
            DateTime now = DateTime.Now;

            StringBuilder s = new StringBuilder();
            s.Append(now.Year.ToString());
            s.Append(now.Month.ToString().PadLeft(2, '0'));
            s.Append(now.Day.ToString().PadLeft(2, '0'));
            s.Append("_");
            s.Append(now.Hour.ToString().PadLeft(2, '0'));
            s.Append(now.Minute.ToString().PadLeft(2, '0'));
            s.Append(now.Second.ToString().PadLeft(2, '0'));
            s.Append("_");
            s.Append(now.Millisecond.ToString().PadLeft(3, '0'));

            return s.ToString();
        }

    }
}
