using System;
using System.Collections.Generic;
using System.Text;

namespace GOES_I
{
    public class UserService : IUserService
    {

        public Dictionary<string, object> GetEndUserProduct(string productName, DateTime timestamp)
        {
            return EndUserProducts.EndUserProducts
                .FromName(productName)
                .Get(QueryService.GetCachePath(timestamp));
        }
    }
}
