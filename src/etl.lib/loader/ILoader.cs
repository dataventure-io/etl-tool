using System.Data;
using etl.lib.util;

namespace etl.lib.loader
{
    public interface ILoader : IArguments
    {
        void load(DataTable data);
    }


}