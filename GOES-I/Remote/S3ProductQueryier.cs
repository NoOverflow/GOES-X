using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Serilog;

namespace GOES_I
{
    public class S3ProductQueryier
    {
        public AmazonS3Client Client { get; set; }

        // TODO Add support for other satellites
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

        /// <summary>
        /// Get a raw product from the S3 bucket
        /// </summary>
        /// <param name="key">S3 Product key</param>
        /// <param name="path">The path to which the product will be saved</param>
        /// <returns></returns>
        public async Task<Product> GetProduct(string key, string path)
        {
            var result = await Client.GetObjectAsync(GOES_16_BUCKET, key);

            Log.Logger.Debug("S3 Query, Key={0} StatusCode={1}", key, result.HttpStatusCode);
            if (!File.Exists(String.Format("{0}", key.Replace("/", "_"))))
                await result.WriteResponseStreamToFileAsync(path, false, new System.Threading.CancellationToken());
            return new Product(String.Format("{0}", path));
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
                retObjects.AddRange(listResponse.S3Objects);
                listRequest.Marker = listResponse.NextMarker;
            } while (listResponse.IsTruncated);
            return retObjects;
        }
    }
}
