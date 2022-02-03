using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GOES_I.EndUserProducts
{
    internal class BandsEndUserProduct : IEndUserProduct
    {
        public string Name { get; set; } = "Bands";

        public GoesProduct[] GetRequirements()
        {
            return new GoesProduct[] {
                GoesProduct.MCMIPF
            };
        }

        private void ProcessRawBands(Product mcmipfProduct, short bandIndex, string storagePath)
        {
            short[,] rawRad = (short[,])mcmipfProduct.InternalDataSet[String.Format("CMI_C{0:00}", bandIndex)].GetData();
            double[,] rad = new double[rawRad.GetLength(0), rawRad.GetLength(1)];
            float radSf = (float)mcmipfProduct.InternalDataSet[String.Format("CMI_C{0:00}", bandIndex)].Metadata["scale_factor"];

            for (int y = 0; y < rad.GetLength(0); y++)
                for (int x = 0; x < rad.GetLength(1); x++)
                    rad[y, x] = rawRad[y, x];
            Utils.ImageUtils.CreateImage(rad, gamma: 1.6, transparencyKey: -1).Save(Path.Combine(storagePath, $"Band{bandIndex}.png"), ImageFormat.Png);
        }

        public bool IsProcessComplete(string basePath)
        {
            string path = Path.Combine(basePath, Name);

            if (!Directory.Exists(path))
                return false;
            for (short i = 1; i <= 16; i++)
            {
                if (!File.Exists(Path.Combine(path, String.Format("Band{0}.png", i))))
                    return false;
            }
            return true;
        }

        public async Task Process(string basePath)
        {
            Product mcmipf;
            string path = Path.Combine(basePath, Name);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            mcmipf = new Product(GoesProduct.MCMIPF, Path.Combine(basePath, "MCMIPF.nc"));
            for (short i = 1; i <= 16; i++)
            {
                ProcessRawBands(mcmipf, i, path);
            }
        }

        public bool HasRequirements(string rawProductsPath)
        {
            return File.Exists(Path.Combine(rawProductsPath, "MCMIPF.nc"));
        }

        public Dictionary<string, object> Get(string rawProductsPath)
        {
            string path = Path.Combine(rawProductsPath, Name);
            Dictionary<string, object> result = new Dictionary<string, object>();

            for (short i = 1; i <= 16; i++)
                result.Add($"Band{i}", Path.Combine(path, $"Band{i}.png"));
            return result;
        }
    }
}
