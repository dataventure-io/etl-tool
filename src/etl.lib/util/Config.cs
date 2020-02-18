using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace etl.lib.util
{
    public class Config
    {
        private static Config _config = null;

        public static Config Current
        {
            get
            {
                return _config;
            }
        }

        string filename = string.Empty;
        protected Dictionary<string, Variable> allReferences = new Dictionary<string, Variable>();

        public class Variable
        {
            public string name = string.Empty;

            public string originalValue = string.Empty;
            protected string _value = string.Empty;

            

            public Dictionary<string, Variable> references = new Dictionary<string, Variable>();

            protected Config config = null;

            bool resolved = false;

            public Variable(Config config, string name, string originalValue)
            {
                this.config = config;
                this.name = name;
                this.originalValue = originalValue;

                if (originalValue.Length > 0)
                {
                    parse();
                }
            }

            public virtual string Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value;
                }
            }

            public void updateOriginalValue(string originalValue)
            {
                this.originalValue = originalValue;
                resolved = false;
                parse();
            }

            void parse()
            {
                char ch;
                char pct = '%';
                int i;
                bool inToken = false;
                StringBuilder token = new StringBuilder();

                for (i = 0; i < originalValue.Length; i++)
                {
                    ch = originalValue.ElementAt(i);

                    if (ch == pct && !inToken)  // start of token
                    {
                        token.Clear();
                        inToken = true;
                    }
                    else if (ch == pct && inToken)  // end of token
                    {
                        Variable v = null;
                        string tokenName = token.ToString();
                        if (config.allReferences.ContainsKey(tokenName))
                        {
                            v = config.allReferences[tokenName];
                        }
                        else
                        {
                            /* so this variable has not been seen before
                             have to decide if this is a forward ref, 
                             a recursive ref, or a ref to an env variable */

                            // 1) check if this is a recursive ref
                            //    a recursive ref is automatically assumed to be 
                            //    an environment variable.
                            if (tokenName.ToLower() == this.name.ToLower())
                            {
                                string origTokenName = tokenName;
                                originalValue = originalValue.Replace("%" + tokenName + "%", "%_" + tokenName + "%");
                                tokenName = "_" + tokenName;

                                if (!config.allReferences.ContainsKey(origTokenName))
                                {
                                    string envValue = Environment.GetEnvironmentVariable(origTokenName);
                                    if (envValue == null) envValue = string.Empty;

                                    v = new Variable(config, tokenName, envValue);
                                    config.allReferences.Add(v.name, v);
                                }

                            }
                            // 2) check if this is a ref to an environment variable
                            else if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(tokenName)))
                            {
                                v = new Variable(config, tokenName, Environment.GetEnvironmentVariable(tokenName));
                            }
                            // 3) this must be a forward ref
                            else
                            {
                                v = new Variable(config, tokenName, string.Empty);
                                config.allReferences.Add(v.name, v);
                            }
                        }

                        if (!references.ContainsKey(tokenName))
                        {
                            references.Add(tokenName, v);
                        }
                        inToken = false;
                    }
                    else if (ch != pct && inToken) // accumulate chars
                    {
                        token.Append(ch);
                    }
                }

            }

            public void eval()
            {
                if (resolved) return;

                if (references.Count == 0)
                {
                    resolved = true;
                    Value = originalValue;
                }
                else
                {
                    Dictionary<string, Variable>.ValueCollection vc = references.Values;
                    Value = originalValue;

                    for (int i = 0; i < vc.Count; i++)
                    {
                        Variable vref = vc.ElementAt(i);

                        if (!vref.resolved) throw new Exception(vref.name + " not defined.");

                        Value = Value.Replace("%" + vref.name + "%", vref.Value);

                    }

                    resolved = true;
                }
            }

        }

        public class TodayVariable : Variable
        {
            DateTime now = DateTime.Now;

            public TodayVariable( Config config, string name, string originalValue ):base(config, name, originalValue)
            {

            }

            public override string Value
            {
                get
                {
                    string dateFormat = config.getValue("DATE_FORMAT");
                    return now.ToString(dateFormat); 
                }
                set
                {
                    _value = value;
                }
            }
                   
        }

        public Config(string filename)
        {
            this.filename = filename;

            macros();
            parse();
            eval();

            _config = this;
        }

        public string BaseName
        {
            get
            {
                return Path.GetFileNameWithoutExtension(filename);
            }
        }

        void macros()
        {
            addToday();
        }

        private void addToday()
        {
            TodayVariable today = new TodayVariable(this, "TODAY", "");
            allReferences.Add(today.name, today);
        }

        void parse()
        {
            string line = string.Empty;

            StreamReader sr = new StreamReader(filename);

            while ((line = sr.ReadLine()) != null)
            {
                if (line.Contains("#")) continue;
                line = line.Trim();
                if (line.Length == 0) continue;

                string originalValue = string.Empty;
                string name = string.Empty;

                line = line.Replace("SET", "");

                line = line.Trim();

                string[] parts = line.Split(new char[] { '=' });

                name = parts[0].Trim();
                originalValue = parts[1].Trim();

                addVariable(name, originalValue);

            }
            
            sr.Close();

        }

        private Variable addVariable(string name, string originalValue)
        {
            Variable v;
            if (allReferences.ContainsKey(name))
            {
                v = allReferences[name];
                v.updateOriginalValue(originalValue);
            }
            else
            {
                v = new Variable(this, name, originalValue);
                allReferences.Add(name, v);
            }

            return v;
        }

        void eval()
        {
            buildTree();
            evalReferences();
        }

        void buildTree()
        {
            Dictionary<string, Variable> tmp = new Dictionary<string, Variable>();

            do
            {
                Dictionary<string, Variable>.ValueCollection vc = allReferences.Values;

                int i = 0;
                while (i < allReferences.Count)
                {
                    Variable v = vc.ElementAt(i++);

                    if (v.references.Count == 0)
                    {
                        if (!tmp.ContainsKey(v.name))
                        {
                            tmp.Add(v.name, v);
                            allReferences.Remove(v.name);
                            i = 0;
                        }
                    }
                    else
                    {
                        Dictionary<string, Variable>.ValueCollection vrc = v.references.Values;
                        bool found = true;
                        for (int j = 0; j < vrc.Count && found; j++)
                        {
                            Variable r = vrc.ElementAt(j);
                            if (tmp.ContainsKey(r.name))
                            {
                                found = true;
                            }
                            else
                            {
                                found = false;
                            }
                        }

                        if (found)
                        {
                            tmp.Add(v.name, v);
                            allReferences.Remove(v.name);
                            i = 0;
                        }
                    }
                }

            } while (allReferences.Count > 0);

            allReferences = tmp;
        }

        void evalReferences()
        {
            Dictionary<string, Variable>.ValueCollection vc = allReferences.Values;

            for (int i = 0; i < vc.Count; i++)
            {
                Variable v = vc.ElementAt(i);

                v.eval();
            }
        }

        public string getValue(string name)
        {
            string result = string.Empty;

            if (allReferences.ContainsKey(name))
            {
                Variable vResult;
                vResult = allReferences[name];

                result = vResult.Value;
            }

            return result;
        }

        public int getCount()
        {
            return allReferences.Count;
        }

        public string getName(int i)
        {
            string name = string.Empty;
            if ((i >= 0) && (i < allReferences.Count))
            {
                KeyValuePair<string, Variable> kvp = allReferences.ElementAt(i);
                name = kvp.Key;
            }

            return name;
        }

        public bool containsKey(string name)
        {
            return allReferences.ContainsKey(name);
        }
    }
}
