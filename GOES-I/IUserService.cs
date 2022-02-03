using System;
using System.Collections.Generic;
using System.Text;

namespace GOES_I
{
    public interface IUserService
    {
        public Dictionary<string, object> GetEndUserProduct(string productName, DateTime timestamp);
    }
}
