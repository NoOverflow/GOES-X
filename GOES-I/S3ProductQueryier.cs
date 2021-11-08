using System;
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

        public async Task ListRawProducts()
        {
            ListObjectsRequest listRequest = new ListObjectsRequest
            {
                BucketName = GOES_16_BUCKET,                
            };
            ListObjectsResponse listResponse;

            do
            {
                listResponse = await Client.ListObjectsAsync(listRequest);
                foreach (S3Object obj in listResponse.S3Objects)
                {
                    Console.WriteLine("Object - " + obj.Key);
                    Console.WriteLine(" Size - " + obj.Size);
                    Console.WriteLine(" LastModified - " + obj.LastModified);
                    Console.WriteLine(" Storage class - " + obj.StorageClass);
                }
                listRequest.Marker = listResponse.NextMarker;
            } while (listResponse.IsTruncated);
        }
    }
}
