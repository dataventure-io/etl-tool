using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using etl.lib.util;

namespace etl.lib.transformer
{
    public class FormatTransformer : AbstractTransformer, ITransformer
    {
        public override DataTable transform(Arguments arg, DataTable data)
        {
            throw new NotImplementedException();
        }
    }
}
