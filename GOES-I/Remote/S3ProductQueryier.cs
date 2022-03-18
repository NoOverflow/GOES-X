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
        public async Task<Product> GetProduct(GoesProduct type, string key, string path)
        {
            var result = await Client.GetObjectAsync(GOES_16_BUCKET, key);

            Log.Logger.Debug("S3 Query, Key={0} StatusCode={1} CachePath={2}", key, result.HttpStatusCode, path);
            result.WriteObjectProgressEvent += Result_WriteObjectProgressEvent;
            if (!File.Exists(path))
                await result.WriteResponseStreamToFileAsync(path, false, new System.Threading.CancellationToken());
            else
                Log.Logger.Warning("Tried to download a file that already existed: {0}", path);
            return new Product(type, path);
        }

        private static int previousProgressV = -1;
        private void Result_WriteObjectProgressEvent(object sender, WriteObjectProgressArgs e)
        {
            if (e.PercentDone % 10 == 0 && (e.PercentDone % 10 != previousProgressV))
                Log.Logger.Information("S3 Object {0} download, progress: {1}%", e.Key, e.PercentDone);
            previousProgressV = e.PercentDone % 10;
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
