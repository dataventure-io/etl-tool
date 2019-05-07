using System.Data;
using etl.lib.util;

namespace etl.lib.transformer
{
    public interface ITransformer
    {
        DataTable transform(Arguments arg, DataTable data);
    }
}