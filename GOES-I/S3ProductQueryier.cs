using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace GOES_I
{
    public class S3ProductQueryier
    {
        public AmazonS3Client Client { get; set; }

        private const string GOES_16_BUCKET = "noaa-goes16";

        public S3ProductQueryier(AmazonS3Client Client)
        {
            this.Client = Client;
        }

        public string GetS3Prefix(string productName, DateTime time)
        {
            return String.Format("{0}/{1}/{2:000}/{3:00}", 
                productName, 
                time.Year, 
                time.DayOfYear,
                time.Hour);
        }

        public async Task<Product> GetProduct(string key)
        {
            key = "ABI-L2-MCMIPF_2021_353_19_OR_ABI-L2-MCMIPF-M6_G16_s20213531900206_e20213531909519_c20213531910018.nc";
            /*var result = await Client.GetObjectAsync(GOES_16_BUCKET, key);

            Console.WriteLine("Raw Product returned: StatusCode:" + result.HttpStatusCode + " Content-Length: " + result.ContentLength);
            if (!File.Exists(String.Format("{0}", key.Replace("/", "_"))))
                await result.WriteResponseStreamToFileAsync(String.Format("{0}", key.Replace("/", "_")), false, new System.Threading.CancellationToken());*/
            return new Product(String.Format("{0}", key.Replace("/", "_")));
        }

        public async Task<List<S3Object>> ListRawProducts(string prefix = "")
        {
            ListObjectsRequest listRequest = new ListObjectsRequest
            {
                BucketName = GOES_16_BUCKET,
                Prefix = prefix
            };
            ListObjectsResponse listResponse;
            List<S3Object> retObjects = new List<S3Object>();

            do
            {
                listResponse = await Client.ListObjectsAsync(listRequest);
                /*foreach (S3Object obj in listResponse.S3Objects)
                {
                    Console.WriteLine("Object - " + obj.Key);
                    Console.WriteLine(" Size - " + obj.Size);
                    Console.WriteLine(" LastModified - " + obj.LastModified);
                    Console.WriteLine(" Storage class - " + obj.StorageClass);
                }*/
                retObjects.AddRange(listResponse.S3Objects);
                listRequest.Marker = listResponse.NextMarker;
            } while (listResponse.IsTruncated);
            return retObjects;
        }
    }
}
