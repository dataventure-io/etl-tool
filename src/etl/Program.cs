using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using etl.lib.util;
using etl.lib.control;
using etl.lib.extractor;
namespace etl
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Arguments arg = CommandLineParser.parse(args);

            Console.WriteLine(arg.getValue( typeof (etl.lib.extractor.ExcelExtractor), "ExcelFile"));

            /*
            Controller controller = new Controller();

            etl.lib.util.Logger.infoEvent += Logger_infoEvent;
            etl.lib.util.Logger.debugEvent += Logger_debugEvent;
            etl.lib.util.Logger.errorEvent += Logger_errorEvent;
           

            controller.execute(arg);

            Console.ReadLine();

            */
        }

        private static void Logger_errorEvent(DateTime dateTime, string message)
        {
            string level = "ERROR";
            writeLog(dateTime, level, message);
        }

        private static void Logger_debugEvent(DateTime dateTime, string message)
        {
            string level = "DEBUG";
            writeLog(dateTime, level, message);
        }

        private static void Logger_infoEvent(DateTime dateTime, string message)
        {
            string level = "INFO";
            writeLog(dateTime, level, message);
        }

        private static void writeLog(DateTime dateTime, string level, string message)
        {
            Console.WriteLine(dateTime.ToShortDateString() + " " + dateTime.ToLongTimeString() + "\t" + level + "\t" + message);
        }
    }
}
