using System;
using GOES_I;
using Amazon.S3;
using Amazon.S3.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using GOES_I.Logging;

namespace GOES_ITest
{
    class Program
    {
        private async Task TestSatelliteStatus()
        {

        }

        private async Task TestColor()
        {
            AmazonS3Client client = new AmazonS3Client(new Amazon.Runtime.AnonymousAWSCredentials(), Amazon.RegionEndpoint.USEast1);
            S3ProductQueryier pqueryier = new S3ProductQueryier(client);
            List<S3Object> rawObjects = await pqueryier.ListRawProducts(pqueryier.GetS3Prefix("ABI-L2-MCMIPF", DateTime.Now.AddDays(-1)));
            string key = rawObjects[0].Key;
            Product product = await pqueryier.GetProduct(GoesProduct.MCMIPF, key, "");

            short[,] rawR = (short[,])product.InternalDataSet["CMI_C02"].GetData();
            short[,] rawG = (short[,])product.InternalDataSet["CMI_C03"].GetData();
            short[,] rawB = (short[,])product.InternalDataSet["CMI_C01"].GetData();
            double[,] r = new double[rawG.GetLength(0), rawG.GetLength(1)];
            double[,] g = new double[rawG.GetLength(0), rawG.GetLength(1)];
            double[,] b = new double[rawG.GetLength(0), rawG.GetLength(1)];
            
            
            float rScaleFactor = (float)product.InternalDataSet["CMI_C02"].Metadata["scale_factor"];
            float bScaleFactor = (float)product.InternalDataSet["CMI_C03"].Metadata["scale_factor"];
            float gScaleFactor = (float)product.InternalDataSet["CMI_C01"].Metadata["scale_factor"];

            for (int y = 0; y < rawG.GetLength(0); y++)
            {
                for (int x = 0; x < rawG.GetLength(1); x++)
                {
                    r[y, x] = (double)(rawR[y, x] * rScaleFactor);
                    b[y, x] = (double)(rawB[y, x] * bScaleFactor);
                    g[y, x] = (double)(0.48358168 * r[y, x] + 0.45706946 * b[y, x] + 0.06038137 * (rawG[y, x] * gScaleFactor));
                }
            }
            GOES_I.Utils.ImageUtils.CreateRGBImage(r, g, b, gamma: 1.8, -1).Save("colored_image.png", ImageFormat.Png);
        }

        static async Task Main(string[] args)
        {
            Logger.Init();
            Program program = new Program();
            AmazonS3Client client = new AmazonS3Client(new Amazon.Runtime.AnonymousAWSCredentials(), Amazon.RegionEndpoint.USEast1);
            QueryService queryService = new QueryService(client);
            IUserService userService = new UserService();
            // queryService.Start();
            var test = userService.GetEndUserProduct("Bands", DateTime.UtcNow);
            Console.ReadLine();
            queryService.Stop();
        }
    }
}
