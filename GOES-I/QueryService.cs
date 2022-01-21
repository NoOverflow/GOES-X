using Amazon.S3;
using Amazon.S3.Model;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GOES_I
{
    /// <summary>
    /// A Query Service used to get data from the AWS S3 Bucket, treat it, and store the results
    /// into the filesystem, this allows us to deliver user-ready data without having to parse it
    /// on every request.
    /// </summary>
    public class QueryService
    {
        /// <summary>
        /// The current query time, this will go back in time as we get more data
        /// </summary>
        public DateTime CurrentQueryTime { get; set; } = DateTime.UtcNow;
        public DateTime StartQueryTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The time span for which the query service will get the data
        /// TODO: Make this a setting
        /// </summary>
        public TimeSpan QueryTimeSpan { get; set; } = TimeSpan.FromDays(10);

        /// <summary>
        /// The timespan added to the CurrentQueryTime on each request, 
        /// Example: TimeSpan(1h) means we will get data for each hour. 
        /// Warning: This must not be inferior to the region interval time (Full disk, Continental, MesoScale...)
        /// </summary>
        public TimeSpan IncrementalTimeSpan { get; set; } = TimeSpan.FromHours(1);

        /// <summary>
        /// The storage path, we don't use a database because the filesystem will do for now.
        /// </summary>
        private string StoragePath;

        private AmazonS3Client AwsClient;

        public QueryService(AmazonS3Client awsClient, string? absoluteStoragePath = null)
        {
            this.AwsClient = awsClient;
            if (absoluteStoragePath == null)
                StoragePath = absoluteStoragePath = AppDomain.CurrentDomain.BaseDirectory + "/storage";
            if (!Directory.Exists(absoluteStoragePath))
                Directory.CreateDirectory(absoluteStoragePath);
        }

        /// <summary>
        /// Returns true if we have ALL data for a specific timestamp
        /// </summary>
        /// <param name="timemark">The timestamp</param>
        /// <returns>True if we have all data</returns>
        private bool IsTimemarkComplete(DateTime timemark)
        {
            string timePath = Path.Combine(StoragePath, 
                String.Format("{0}/{1}/{2}", 
                    timemark.Year, 
                    timemark.DayOfYear, 
                    timemark.Hour));

            if (!Directory.Exists(StoragePath + "/" + timePath + "/"))
                return false;
            // TODO: We can't do that, some products are only available at different intervals
            // 
            /*foreach (GoesChannel product in Enum.GetValues(typeof(GoesChannel)))
            {
                if (!File.Exists(StoragePath + "/" + timePath + "/" + product.ToString() + ".png"))
                {
                    Log.Logger.Warning("QueryService: Directory exists but incomplete data from {0} {1}, downloading missing raw data",
                        CurrentQueryTime.ToShortDateString(),
                        CurrentQueryTime.ToShortTimeString());
                    return false;
                }
            }*/
            return true;
        }

        private async Task<bool> TimemarkExists(string product, DateTime timemark)
        {
            string path = BuildS3Path(product, timemark);
            ListObjectsResponse response = await AwsClient.ListObjectsAsync(path);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                return false;
            return response.S3Objects.Count > 0;
        }

        private string BuildS3Path(string product, DateTime timestamp)
        {
            return $"{product}/{timestamp.Year}/{timestamp.DayOfYear}/{timestamp.Hour}/";
        }

        public void Start()
        {
            // TODO: Move to a thread
            QueryLogic(new CancellationToken());
        }

        public void Stop()
        {

        }

        private async void QueryLogic(CancellationToken cancellationToken)
        {
            // Backtrack from current -> current - QueryTimeSpan
            while (CurrentQueryTime >= StartQueryTime - QueryTimeSpan)
            {
                if (!IsTimemarkComplete(CurrentQueryTime))
                {
                    Log.Logger.Information("QueryService: Missing data from {0} {1}, downloading raw data", 
                        CurrentQueryTime.ToShortDateString(), 
                        CurrentQueryTime.ToShortTimeString());
                }
                CurrentQueryTime -= IncrementalTimeSpan;
            }
            Log.Logger.Information("QueryService: Now querying products since {0}",
                StartQueryTime.ToShortDateString() + " - " + StartQueryTime.ToShortTimeString());
            CurrentQueryTime = StartQueryTime;
            while (!cancellationToken.IsCancellationRequested)
            {
                while (CurrentQueryTime < DateTime.UtcNow + IncrementalTimeSpan)
                {
                    bool tmExists = await TimemarkExists("ABI-L1b-RadC", CurrentQueryTime);

                    if (!IsTimemarkComplete(CurrentQueryTime) && tmExists)
                    {
                        Log.Logger.Information("QueryService: Missing data from {0} {1}, downloading raw data",
                            CurrentQueryTime.ToShortDateString(),
                            CurrentQueryTime.ToShortTimeString()
                        );
                        // Download it
                        CurrentQueryTime += IncrementalTimeSpan;
                    } 
                    else
                    {
                        // Move to a config
                        Thread.Sleep(1000);
                    }
                }
                // Move to a config
                Thread.Sleep(1000);
            }
            // Once done, wait for new data to come, and get them
        }
    }
}
