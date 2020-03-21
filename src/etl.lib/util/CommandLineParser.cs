using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace etl.lib.util
{
    public class CommandLineParser
    {
        protected static string usage = "etl  <configfile>";

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
                    string filename = args[i++];

                    arguments = loadConfig( filename );
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
            return  Arguments.loadConfig(filename);
        }

       
    }


}
