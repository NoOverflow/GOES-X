using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GOES_I.EndUserProducts
{
    internal static class EndUserProducts
    {
        public static IEndUserProduct[] EndUserProductsArray = new IEndUserProduct[]
        {
            new BandsEndUserProduct()
        };

        public static IEndUserProduct FromName(string name)
        {
            return EndUserProductsArray.First(x => x.Name == name);
        }
    }
}
