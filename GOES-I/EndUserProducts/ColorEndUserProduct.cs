using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GOES_I.EndUserProducts
{
    internal class ColorEndUserProduct : IEndUserProduct
    {
        public string Name { get; set; } = "Color";

        public GoesProduct[] GetRequirements()
        {
            return new GoesProduct[] {
                GoesProduct.MCMIPF
            };
        }

        public bool IsProcessComplete(string basePath)
        {
            string path = Path.Combine(basePath, Name);

            if (!Directory.Exists(path))
                return false;
            if (!File.Exists(Path.Combine(path, "Color.png")))
                return false;
            return true;
        }

        public async Task Process(string basePath)
        {
            Product mcmipf;
            string path = Path.Combine(basePath, Name);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            mcmipf = new Product(GoesProduct.MCMIPF, Path.Combine(basePath, "MCMIPF.nc"));

            short[,] rawR = (short[,])mcmipf.InternalDataSet["CMI_C02"].GetData();
            short[,] rawG = (short[,])mcmipf.InternalDataSet["CMI_C03"].GetData();
            short[,] rawB = (short[,])mcmipf.InternalDataSet["CMI_C01"].GetData();

            double[,] r = new double[rawG.GetLength(0), rawG.GetLength(1)];
            double[,] g = new double[rawG.GetLength(0), rawG.GetLength(1)];
            double[,] b = new double[rawG.GetLength(0), rawG.GetLength(1)];

            float rScaleFactor = (float)mcmipf.InternalDataSet["CMI_C02"].Metadata["scale_factor"];
            float bScaleFactor = (float)mcmipf.InternalDataSet["CMI_C03"].Metadata["scale_factor"];
            float gScaleFactor = (float)mcmipf.InternalDataSet["CMI_C01"].Metadata["scale_factor"];

            for (int y = 0; y < rawG.GetLength(0); y++)
            {
                for (int x = 0; x < rawG.GetLength(1); x++)
                {
                    r[y, x] = (double)(rawR[y, x] * rScaleFactor);
                    b[y, x] = (double)(rawB[y, x] * bScaleFactor);
                    g[y, x] = (double)(0.48358168 * r[y, x] + 0.45706946 * b[y, x] + 0.06038137 * (rawG[y, x] * gScaleFactor));
                }
            }
            GOES_I.Utils.ImageUtils.CreateRGBImage(r, g, b, gamma: 1.8, -1).Save(Path.Combine(path, $"Color.png"), ImageFormat.Png);
        }

        public bool HasRequirements(string rawProductsPath)
        {
            return File.Exists(Path.Combine(rawProductsPath, "MCMIPF.nc"));
        }

        public Dictionary<string, object> Get(string rawProductsPath)
        {
            string path = Path.Combine(rawProductsPath, Name);
            Dictionary<string, object> result = new Dictionary<string, object>();

            result.Add("Color", Path.Combine(path, "Color.png"));
            return result;
        }
    }
}
