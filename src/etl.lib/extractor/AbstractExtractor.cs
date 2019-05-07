using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using etl.lib.util;

namespace etl.lib.extractor
{
    public class AbstractExtractor : IExtractor
    {
        protected Arguments arguments = null;

        public virtual DataTable extract(Arguments arg)
        {
            this.arguments = arg;
            return null;
        }
    }
}
