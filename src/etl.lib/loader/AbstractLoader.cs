using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using etl.lib.util;

namespace etl.lib.loader
{
    public class AbstractLoader : ILoader
    {
        protected Arguments arguments = null;

        public AbstractLoader()
        {

        }

        public AbstractLoader(Arguments arg)
        {
            setArgs(arg);
        }

        public virtual void load( DataTable data)
        {
        }

        public void setArgs(Arguments arg)
        {
            this.arguments = arg;
        }
    }
}
