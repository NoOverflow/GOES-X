using System;

namespace GOES_X.Model
{
    public enum GoesProduct
    {
        Blue = 1,
        Red = 2,
        Veggie = 3,
        Cirrus = 4,
        Snow = 5,
        CloudParticleSize = 6,
        Shortwave = 7,
        UprLevelWaterVapor = 8,
        MidLevelWaterVapor = 9,
        LwrLevelWaterVapor = 10,
        CloudTopPhase = 11,
        Ozone = 12,
        CleanLWW = 13,
        LWW = 14,
        DirtyLWW = 15,
        CO2LWW = 16
    }

    [Serializable]
    public class Product
    {
        /// <summary>
        /// The GOES Product index based on GOES-R series satellite specifications
        /// </summary>
        public GoesProduct ProductIndex { get; set; }

        /// <summary>
        /// User-friendly product name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The current opacity for this product, unused if not currently displayed by the user
        /// </summary>
        public double Opacity { get; set; }

        /// <summary>
        /// Best spatial resolution for this product, expressed with a factor (0.5x, 2x...)
        /// </summary>
        public float BestSpatialResolution { get; set; }

        /// <summary>
        /// The central wavelength of this product (in nm)
        /// </summary>
        public float CentralWaveLength { get; set; }
    }
}
