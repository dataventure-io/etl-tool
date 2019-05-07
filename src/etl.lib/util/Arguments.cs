using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace etl.lib.util
{
    public class Arguments
    {
        System.Collections.Generic.Dictionary<string, string> arguments = new Dictionary<string, string>();

        public void addArgument( string name, string value )
        {
            arguments.Add(name, value);
        }


        public string getValue(string key)
        {
            string val = string.Empty;

            if (arguments.ContainsKey(key))
            {
                val = arguments[key];
            }

            return val;
        }

        public string getValue(Type t, string m)
        {
            string val = string.Empty;

            if (m.StartsWith("get_"))
            {
                m = m.Replace("get_", "");
            }

            string key = t.Name + "." + m;

            if (arguments.ContainsKey(key))
            {
                val = arguments[key];
            }

            return val;
        }
    }
}
