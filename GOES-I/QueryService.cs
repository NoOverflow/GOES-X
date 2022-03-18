using Amazon.S3;
using Amazon.S3.Model;
using GOES_I.EndUserProducts;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public class QueryService : IQueryService
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
        public TimeSpan QueryTimeSpan { get; set; } = TimeSpan.FromHours(2);

        /// <summary>
        /// The timespan added to the CurrentQueryTime on each request, 
        /// Example: TimeSpan(1h) means we will get data for each hour. 
        /// Warning: This must not be inferior to the region interval time (Full disk, Continental, MesoScale...)
        /// </summary>
        public TimeSpan IncrementalTimeSpan { get; set; } = TimeSpan.FromHours(1);

        /// <summary>
        /// The storage path, we don't use a database because the filesystem will do for now.
        /// </summary>
        public static string StoragePath;

        private AmazonS3Client AwsClient;

        private S3ProductQueryier ProductQueryier { get; set; }

        /// <summary>
        /// The thread hosting the query logic task
        /// </summary>
        private Thread QueryLogicThread = null;

        /// <summary>
        /// The cancellation token for the query logic
        /// </summary>
        private CancellationTokenSource QueryLogicCancellationTokenSource = new CancellationTokenSource();

        public QueryService(AmazonS3Client awsClient, string? absoluteStoragePath = null)
        {
            this.AwsClient = awsClient;
            this.ProductQueryier = new S3ProductQueryier(this.AwsClient);
            
            if (absoluteStoragePath == null)
                StoragePath = absoluteStoragePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "storage");
            else
                StoragePath = absoluteStoragePath;
            if (!Directory.Exists(absoluteStoragePath))
                Directory.CreateDirectory(absoluteStoragePath);
        }

        /// <summary>
        /// Returns true if we have ALL data for a specific timestamp
        /// </summary>
        /// <param name="timemark">The timestamp</param>
        /// <returns>True if we have all data</returns>
        private async Task<bool> IsCacheCompleteAsync(DateTime timemark)
        {
            try
            {
                string hDirectory = GetCachePath(timemark);
                IEnumerable<string> mDirectories = null;
                string path = String.Empty;
                List<S3Object> objects = null;

                if (!Directory.Exists(hDirectory))
                    return false;
                mDirectories = Directory.EnumerateDirectories(hDirectory);
                path = ProductQueryier.GetS3Prefix("ABI-L2-MCMIPF", timemark); // TODO: Do it for every requirements
                objects = await ProductQueryier.ListRawProducts(path);
                foreach (var obj in objects)
                    if (mDirectories.Count(x => Convert.ToInt32(x) == obj.LastModified.Minute) == 0)
                        return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }         
        }

        private async Task<bool> TimemarkExists(string product, DateTime timemark)
        {
            try
            {
                string path = ProductQueryier.GetS3Prefix(product, timemark);
                List<S3Object> objects = await ProductQueryier.ListRawProducts(path);

                return objects.Count > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetCachePath(DateTime timemark)
        {
            return Path.Combine(StoragePath,
                String.Format("{0}/{1}/{2}",
                    timemark.Year,
                    timemark.DayOfYear,
                    timemark.Hour));
        }

        private async Task TimemarkCacheDownload(DateTime timemark)
        {
            // TODO: Add other NOAA products from EUPs requirements
            var objects = await ProductQueryier.ListRawProducts(ProductQueryier.GetS3Prefix("ABI-L2-MCMIPF", timemark));
            
            if (objects.Count == 0)
            {
                Log.Logger.Warning("QueryService: Tried to cache an empty product at timemark: {0}", timemark);
                return;
            }

            foreach (S3Object obj in objects)
            {
                string cachePath = Path.Combine(GetCachePath(timemark), obj.LastModified.Minute.ToString());

                if (Directory.Exists(cachePath))
                {
                    Log.Logger.Debug("QueryService: Already got product at {0} - {1}", obj.LastModified.ToShortDateString(), obj.LastModified.ToShortTimeString());
                    continue;
                }
                await ProductQueryier.GetProduct(GoesProduct.MCMIPF, obj.Key, Path.Combine(cachePath, "MCMIPF.nc"));
                foreach (var eup in EndUserProducts.EndUserProducts.EndUserProductsArray)
                    await eup.Process(cachePath);
                Log.Logger.Information("EndUserProductService: Processed end user products for {0} - {1}", obj.LastModified.ToShortDateString(), obj.LastModified.ToShortTimeString());
            }      
        }

        public void Start()
        {
            QueryLogicThread = new Thread(async () =>
            {
                await QueryLogic(QueryLogicCancellationTokenSource.Token);
            });

            QueryLogicThread.Priority = ThreadPriority.BelowNormal;
            QueryLogicThread.Name = "Query Logic Thread";
            QueryLogicThread.Start();
        }

        public void Stop()
        {
            QueryLogicCancellationTokenSource.Cancel();
            QueryLogicThread.Abort();
        }

        private async Task QueryLogic(CancellationToken cancellationToken)
        {
            // Backtrack from current -> current - QueryTimeSpan
            while (CurrentQueryTime >= StartQueryTime - QueryTimeSpan)
            {
                // TODO: Check for all EUPs
                bool tmExists = await TimemarkExists("ABI-L2-MCMIPF", CurrentQueryTime);

                if (!tmExists)
                {
                    Log.Logger.Information("QueryService: No data for timemark: {0}, skipping it.", CurrentQueryTime);
                    CurrentQueryTime -= IncrementalTimeSpan;
                    continue;
                }

                // Check for incomplete NetCDF parsing process
                foreach (var eup in EndUserProducts.EndUserProducts.EndUserProductsArray)
                {
                    string hourCachePath = GetCachePath(CurrentQueryTime);

                    if (!Directory.Exists(hourCachePath))
                        continue;
                    IEnumerable<string> minDirectories = Directory.EnumerateDirectories(hourCachePath);

                    foreach (string dir in minDirectories)
                    {
                        if (eup.HasRequirements(dir)
                        && !eup.IsProcessComplete(dir))
                        {
                            Log.Logger.Warning("EndUserProductService: Has requirements but process incomplete. Resuming.");
                            try
                            {
                                await eup.Process(dir);
                            }
                            catch (Exception)
                            {
                                Log.Logger.Warning("EndUserProductService: Corrupted NetCDF file, recovering...");
                                Directory.Delete(dir, true);
                            }
                        }
                    }
                }

                if (!(await IsCacheCompleteAsync(CurrentQueryTime)))
                {
                    Log.Logger.Information("QueryService: Missing data from {0} - {1}, downloading raw data", 
                        CurrentQueryTime.ToShortDateString(), 
                        CurrentQueryTime.ToShortTimeString());
                    await TimemarkCacheDownload(CurrentQueryTime);
                } 
                else
                {
                    Log.Logger.Information("QueryService: Got all data for timemark: {0}!", CurrentQueryTime);
                }
                CurrentQueryTime -= IncrementalTimeSpan;
            }

            Log.Logger.Information("QueryService: Now querying products since {0}",
                StartQueryTime.ToShortDateString() + " - " + StartQueryTime.ToShortTimeString());

            CurrentQueryTime = StartQueryTime + IncrementalTimeSpan;
            while (!cancellationToken.IsCancellationRequested)
            {
                while (CurrentQueryTime < DateTime.UtcNow + IncrementalTimeSpan)
                {
                    // TODO: Check for all EUPs
                    bool tmExists = await TimemarkExists("ABI-L2-MCMIPF", CurrentQueryTime);

                    if (!(await IsCacheCompleteAsync(CurrentQueryTime)) && tmExists)
                    {
                        Log.Logger.Information("QueryService: Missing data from {0} - {1}, downloading raw data",
                            CurrentQueryTime.ToShortDateString(),
                            CurrentQueryTime.ToShortTimeString()
                        );
                        await TimemarkCacheDownload(CurrentQueryTime);
                        CurrentQueryTime += IncrementalTimeSpan;
                    } 
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
                Thread.Sleep(1000);
            }
        }
    }
}
