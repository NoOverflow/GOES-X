using System;
using GOES_I;
using Amazon.S3;
using Amazon.S3.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GOES_ITest
{
    class Program
    {
        private async Task Test()
        {
            AmazonS3Client client = new AmazonS3Client(new Amazon.Runtime.AnonymousAWSCredentials(), Amazon.RegionEndpoint.USEast1);
            S3ProductQueryier pqueryier = new S3ProductQueryier(client);
            List<S3Object> rawObjects = await pqueryier.ListRawProducts(pqueryier.GetS3Prefix("ABI-L1b-RadF", DateTime.Now.AddDays(-1)));
            // Get the first product from band 01
            string key = rawObjects.Find(x => x.Key.Contains("C01")).Key;
            
            await pqueryier.GetRawProduct(key);
        }

        static void Main(string[] args)
        {
            new Program().Test().Wait();
        }
    }
}
