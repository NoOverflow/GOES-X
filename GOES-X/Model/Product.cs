using GOES_I;
using System;

namespace GOES_X.Model
{
    [Serializable]
    public class Product
    {
        /// <summary>
        /// The GOES Product index based on GOES-R series satellite specifications
        /// </summary>
        public GoesProductFullDisk ProductIndex { get; set; }

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
