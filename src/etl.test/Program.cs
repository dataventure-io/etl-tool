using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using etl.lib.util;
using etl.lib.control;

namespace etl_test
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseCfgPath = @"..\..\..\..\config\";
            string configFileName = string.Empty;

            etl.lib.util.Logger.infoEvent += Logger_infoEvent;
            etl.lib.util.Logger.debugEvent += Logger_debugEvent;
            etl.lib.util.Logger.errorEvent += Logger_errorEvent;

            /*runTest(baseCfgPath + "test_int_xlsx_to_sql.cfg");
            runTest(baseCfgPath + "test_names_xlsx_to_sql.cfg");*/
            runTest(baseCfgPath + "test_names_sql_to_sql.cfg");

            Console.ReadLine();
        }

        private static void runTest(string configFileName)
        {
            Arguments testOneArgs = null;

            testOneArgs = CommandLineParser.loadConfig(configFileName);

            Controller controller = new Controller();

            controller.execute(testOneArgs);
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
