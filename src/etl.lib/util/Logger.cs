using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace etl.lib.util
{
    public class Logger
    {
        public delegate void LogEvent(DateTime dateTime, string message);

        public static event LogEvent infoEvent;
        public static event LogEvent debugEvent;
        public static event LogEvent errorEvent;


        public static void info(string message)
        {
            write(infoEvent, message);
        }

        public static void error(string message)
        {
            write(errorEvent, message);
        }

        public static void debug(string message)
        {
            write(debugEvent, message);
        }

        private static void write(LogEvent logEvent, string message)
        {
            if (logEvent == null) return;

            foreach (LogEvent le in logEvent.GetInvocationList())
            {
                le(DateTime.Now, message);
            }
        }
    }
}
