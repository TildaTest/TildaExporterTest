using System;
using System.Collections.Generic;
using System.Text;

namespace TildaExporter
{
    public class TildaResult<T>
    {
        public string status { get; set; }
        public T result { get; set; }
    }
}
