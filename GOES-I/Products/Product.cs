using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using GOES_I.Utils;
using Microsoft.Research;
using Microsoft.Research.Science.Data.NetCDF4;
using Serilog;

namespace GOES_I
{
    public class Product
    {
        public NetCDFDataSet InternalDataSet { get; set; }

        public Product(string path)
        {
            this.InternalDataSet = new NetCDFDataSet(path);

            Log.Logger.Debug("Successfully loaded product,\n\t- Name: {0}\n\t- Dimensions array count: {1}\n\t- Variables array count: {2}",
                this.InternalDataSet.Name,
                this.InternalDataSet.Dimensions.Count,
                this.InternalDataSet.Variables.Count);
        }
    }
}
