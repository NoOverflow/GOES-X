using System;

namespace GOES_X.Model
{
    public enum GeographicCoverage
    {
        CONUS
    }

    [Serializable]
    public class Product
    {
        public double Opacity { get; set; }
    }

    [Serializable]
    public class DataTimings
    {
        public bool Realtime { get; set; }
        public DateTime StartTime { get; set;}
        public DateTime EndTime { get; set; }
    }

    [Serializable]
    public class Animation
    {
        public uint NumberOfFrames {get; set;}
        public TimeSpan IntraFrameTime {get; set;}
    }

    [Serializable]
    public class RuntimeSettings
    {
        public GeographicCoverage GeographicCoverage { get; set; } = GeographicCoverage.CONUS;
        public DataTimings DataTimings {get; set;} = new DataTimings();
        public Animation Animation {get; set;} = new Animation();

    }
}
