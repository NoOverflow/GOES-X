using GOES_I;
using System;

namespace GOES_X.Model
{

    [Serializable]
    public class EndUserProduct
    {
        // TODO: Get this from an API
        public static EndUserProduct[] Products { get; set; } = new EndUserProduct[]
        {
            new EndUserProduct()
            {
                Name = "Band 1 (MCMIPF)",
                BestSpatialResolution = 2.0F,
                CentralWaveLength = 0.47F,
                EupName = "Bands",
                EupIndexName = "Band1"
            },
            new EndUserProduct()
            {
                Name = "Band 2 (MCMIPF)",
                BestSpatialResolution = 2.0F,
                CentralWaveLength = 0.64F,
                EupName = "Bands",
                EupIndexName = "Band2"
            },
            new EndUserProduct()
            {
                Name = "Band 3 (MCMIPF)",
                BestSpatialResolution = 2.0F,
                CentralWaveLength = 0.86F,
                EupName = "Bands",
                EupIndexName = "Band3"
            },
            new EndUserProduct()
            {
                Name = "Band 4 (MCMIPF)",
                BestSpatialResolution = 2.0F,
                CentralWaveLength = 1.37F,
                EupName = "Bands",
                EupIndexName = "Band4"
            },
            new EndUserProduct()
            {
                Name = "Band 5 (MCMIPF)",
                BestSpatialResolution = 2.0F,
                CentralWaveLength = 1.6F,
                EupName = "Bands",
                EupIndexName = "Band5"
            },
            new EndUserProduct()
            {
                Name = "Band 6 (MCMIPF)",
                BestSpatialResolution = 2.0F,
                CentralWaveLength = 2.2F,
                EupName = "Bands",
                EupIndexName = "Band6"
            },
            new EndUserProduct()
            {
                Name = "Band 7 (MCMIPF)",
                BestSpatialResolution = 2.0F,
                CentralWaveLength = 3.9F
                ,
                EupName = "Bands",
                EupIndexName = "Band7"
            },
            new EndUserProduct()
            {
                Name = "Band 8 (MCMIPF)",
                BestSpatialResolution = 2.0F,
                CentralWaveLength = 0.0F,
                EupName = "Bands",
                EupIndexName = "Band8"

            },
            new EndUserProduct()
            {
                Name = "Band 9 (MCMIPF)",
                BestSpatialResolution = 2.0F,
                CentralWaveLength = 0.0F,
                EupName = "Bands",
                EupIndexName = "Band9"
            },
            new EndUserProduct()
            {
                Name = "Band 10 (MCMIPF)",
                BestSpatialResolution = 2.0F,
                CentralWaveLength = 0.0F,
                EupName = "Bands",
                EupIndexName = "Band10"
            },
            new EndUserProduct()
            {
                Name = "Band 11 (MCMIPF)",
                BestSpatialResolution = 2.0F,
                CentralWaveLength = 0.0F,
                EupName = "Bands",
                EupIndexName = "Band11"
            },
            new EndUserProduct()
            {
                Name = "Band 12 (MCMIPF)",
                BestSpatialResolution = 2.0F,
                CentralWaveLength = 0.0F,
                EupName = "Bands",
                EupIndexName = "Band12"
            },
            new EndUserProduct()
            {
                Name = "Band 13 (MCMIPF)",
                BestSpatialResolution = 2.0F,
                CentralWaveLength = 0.0F,
                EupName = "Bands",
                EupIndexName = "Band13"
            },
            new EndUserProduct()
            {
                Name = "Band 14 (MCMIPF)",
                BestSpatialResolution = 2.0F,
                CentralWaveLength = 0.0F,
                EupName = "Bands",
                EupIndexName = "Band14"
            },
            new EndUserProduct()
            {
                Name = "Band 15 (MCMIPF)",
                BestSpatialResolution = 2.0F,
                CentralWaveLength = 0.0F,
                EupName = "Bands",
                EupIndexName = "Band15"
            },
            new EndUserProduct()
            {
                Name = "Band 16 (MCMIPF)",
                BestSpatialResolution = 2.0F,
                CentralWaveLength = 0.0F,
                EupName = "Bands",
                EupIndexName = "Band16"
            },
            new EndUserProduct()
            {
                Name = "Fake Colors RGB (MCMIPF)",
                BestSpatialResolution = 2.0F,
                CentralWaveLength = 0.0F,
                EupName = "Color",
                EupIndexName = "Color"
            }
        };

        /// <summary>
        /// User-friendly product name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The EUP Name used in the API request
        /// </summary>
        public string EupName { get; set; }

        /// <summary>
        /// Internal EUP index, EUPs expose keys that we can access depending on the data we want
        /// </summary>
        public string EupIndexName { get; set; }

        /// <summary>
        /// The current opacity for this product, unused if not currently displayed by the user
        /// </summary>
        public double Opacity { get; set; } = 0.0f;

        /// <summary>
        /// Best spatial resolution for this product, expressed with a factor (0.5x, 2x...)
        /// </summary>
        public float BestSpatialResolution { get; set; }

        /// <summary>
        /// The central wavelength of this product (in microm)
        /// </summary>
        public float CentralWaveLength { get; set; }
    }
}
