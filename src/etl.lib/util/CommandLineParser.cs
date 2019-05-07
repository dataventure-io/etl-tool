using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace etl.lib.util
{
    public class CommandLineParser
    {
        protected static string usage = "etl -cfg <configfile> | -extractor classname=Excel excelfile=<excelfile> sheetname=<sheetname> [-transformer classname=Format filename=<filename>] -loader classname=SqlServer server=<server> database=<database> table=<table>";

        public static string Usage {  get { return usage; } }
        public static Arguments  parse( string[] args )
        {
            Arguments arguments = new Arguments();
            string module = string.Empty;
            try
            {
                int i = 0;

                while (i < args.Length)
                {
                    string token = args[i++];

                    if (token.StartsWith("-"))
                    {
                        if ((token.Equals("-cfg")) || (token.Equals("-config")))
                        {
                            loadConfig(arguments, args[i++]);
                        }
                        else
                        {
                            module = token.Substring(1, token.Length - 1);
                        }
                    }
                    else
                    {
                        string[] parts = token.Split(new char[] { '=' });
                        string paramName = parts[0];
                        string paramValue = parts[1];

                        arguments.addArgument(module + "_" + paramName, paramValue);
                    }
                }
            }
            catch (Exception x)
            {
                throw new EtlException("Usage: " + usage, x);
            }

            return arguments;
        }

        public static Arguments loadConfig( string filename )
        {
            Arguments arguments = new Arguments();
            loadConfig(arguments, filename);
            return arguments;
        }

        protected static void loadConfig( Arguments arguments, string filename )
        {
            string line = string.Empty;
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(filename);
                while ((line = file.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    line = line.Trim();

                    if (line.StartsWith("#")) continue;

                    string[] parts = line.Split(new char[] { '=' });
                    string paramName = parts[0];
                    string paramValue = parts[1];

                    arguments.addArgument(paramName.Trim(), paramValue?.Trim());
                }

                file.Close();
            }
            catch(Exception x)
            {
                throw new EtlException("Error reading " + filename, x);
            }
        }
    }


}
