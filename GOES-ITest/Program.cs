using System;
using GOES_I;
using Amazon.S3;
using Amazon.S3.Model;

namespace GOES_ITest
{
    class Program
    {
        static void Main(string[] args)
        {
            AmazonS3Client client = new AmazonS3Client(new Amazon.Runtime.AnonymousAWSCredentials(), Amazon.RegionEndpoint.USEast1);
            S3ProductQueryier pqueryier = new S3ProductQueryier(client);

            pqueryier.ListRawProducts().Wait();
        }
    }
}
