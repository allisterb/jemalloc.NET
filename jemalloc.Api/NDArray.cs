using System;
using System.Collections.Generic;
using System.Text;

namespace jemalloc
{
    public class NDArray<T> where T : struct
    {
        public NDArray(ulong points, int rank)
        {
            //data[3L]
        }

        HugeArray<T>[] data;
        
    }
}
