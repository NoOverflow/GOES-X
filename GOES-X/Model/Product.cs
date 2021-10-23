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

        public string Name { get; set; }
        public double Opacity { get; set; }
        public float BestSpatialResolution { get; set;}
        public float CentralWaveLength { get; set;}
    }
}
