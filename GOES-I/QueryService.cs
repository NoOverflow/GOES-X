using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GOES_I
{
    public class QueryService
    {
        public QueryService(string relativeStoragePath = "/storage")
        {
            if (!Directory.Exists(relativeStoragePath))
                Directory.CreateDirectory(relativeStoragePath);
        }

        public void Start()
        {

        }

        public void Stop()
        {

        }
    }
}
