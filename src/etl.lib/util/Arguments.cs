using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace etl.lib.util
{
    public class Arguments 
    {
        Dictionary<string,object> arguments = null;


        public string getValue(Type t, string m)
        {
            string val = string.Empty;

            if (m.StartsWith("get_"))
            {
                m = m.Replace("get_", "");
            }

            string key = t.Name ;

            if (arguments.ContainsKey(key))
            {
                Dictionary<string,object> members = (Dictionary<string, object>) arguments[key];

                if (members.ContainsKey(m))
                {
                    val = (string) members[m];
                }
            }

            return val;
        }

        public static Arguments loadConfig(string filename )
        {
            Arguments arg = new Arguments();

            System.IO.StreamReader file =   new System.IO.StreamReader(filename);
            string jsonText = file.ReadToEnd();
            file.Close();

            arg.arguments = (Dictionary<string, object>) deserialize(jsonText);

            return arg;
        }

        protected  static object deserialize(string json)
        {
            return toObject(JToken.Parse(json));
        }

        protected  static object toObject(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    return token.Children<JProperty>()
                                .ToDictionary(prop => prop.Name,
                                              prop => toObject(prop.Value));

                case JTokenType.Array:
                    return token.Select(toObject).ToList();

                default:
                    return ((JValue)token).Value;
            }
        }
    }
}
