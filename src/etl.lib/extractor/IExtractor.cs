﻿using System.Data;
using etl.lib.util;

namespace etl.lib.extractor
{
    public interface IExtractor
    {
        DataTable extract(Arguments arg);
    }
}