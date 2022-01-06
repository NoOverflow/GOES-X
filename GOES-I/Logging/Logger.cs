using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace GOES_I.Logging
{
    public static class Logger
    {
        public static void Init()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("goes_i_log.txt")
                .CreateLogger();
            Log.Logger.Information("Started GOES-I, {0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
        }
    }
}
