using System;
using static GOES_X.Services.UserPreferencesService;

namespace GOES_X.Model
{
    public enum GeographicCoverage
    {
        CONUS
    }

    [Serializable]
    public class DataTimings
    {
        public bool Realtime { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; } = DateTime.Now;
    }

    [Serializable]
    public class Animation
    {
        public uint NumberOfFrames { get; set; }
        public TimeSpan IntraFrameTime { get; set; }
    }

    [Serializable]
    public class UserPreferences
    {
        public GeographicCoverage GeographicCoverage { get; set; } = GeographicCoverage.CONUS;
        public DataTimings DataTimings { get; set; } = new DataTimings();
        public Animation Animation { get; set; } = new Animation();
        public List<EndUserProduct> SelectedProducts { get; set; } = new List<EndUserProduct>();
    }
}
