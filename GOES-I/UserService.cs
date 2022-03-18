using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GOES_I
{
    public class UserService : IUserService
    {

        public Dictionary<string, object> GetEndUserProduct(string productName, DateTime timestamp)
        {
            EndUserProducts.IEndUserProduct eup = EndUserProducts.EndUserProducts.FromName(productName);
            string hCachePath = QueryService.GetCachePath(timestamp);
            IEnumerable<string> minDirectories = Directory.EnumerateDirectories(hCachePath);
            string closestMinPath = String.Empty;
            int closestMinOffset = 0;

            // Get closest available timestamp eup
            foreach (var dir in minDirectories)
            {
                int minute = Convert.ToInt32(Path.GetDirectoryName(dir));

                if (closestMinPath == String.Empty || Math.Abs(timestamp.Minute - minute) < closestMinOffset)
                {
                    closestMinOffset = Math.Abs(timestamp.Minute - minute);
                    closestMinPath = dir; 
                }
            }
            if (closestMinPath == String.Empty)
                throw new Exception("No maching product for this timestamp");
            return eup.Get(closestMinPath);
        }
    }
}
