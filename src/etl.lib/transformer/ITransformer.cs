using System.Data;
using etl.lib.util;

namespace etl.lib.transformer
{
    public interface ITransformer : IArguments
    {
        DataTable transform( DataTable data);
    }
}