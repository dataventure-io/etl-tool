using System.Data;
using etl.lib.util;

namespace etl.lib.loader
{
    public interface ILoader
    {
        void load(Arguments arg, DataTable data);
    }


}