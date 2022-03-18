using System;
using System.Collections.Generic;
using System.Text;

namespace GOES_I
{
    public interface IQueryService
    {
        /// <summary>
        /// Start the query service in a new Thread
        /// </summary>
        public void Start();

        /// <summary>
        /// Stops the started thread
        /// </summary>
        public void Stop();
    }
}
