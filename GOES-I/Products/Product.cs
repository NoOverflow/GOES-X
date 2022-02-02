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
    public enum GoesProduct
    {
        // ABI L1b
        RadC,
        RadF,
        RadM,

        // ABI L2
        ACHAC,
        ACHAF,
        ACHAM,
        ACHTF,
        ACHTM,
        ACMC,
        ACMF,
        ACMM,
        ACTPC,
        ACTPF,
        ACTPM,
        ADPC,
        ADPF,
        ADPM,
        AICEF,
        AITAF,
        AODC,
        AODF,
        BRFC,
        BRFF,
        BRFM,
        CMIPC,
        CMIPF,
        CMIPM,
        MCMIPC,
        MCMIPF,
        MCMIPM,
    }

    public enum GoesChannel
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

    public class Product
    {
        public NetCDFDataSet InternalDataSet { get; set; }
        public GoesProduct GoesProductFullDisk { get; set; }

        public Product(GoesProduct type, string path)
        {
            this.InternalDataSet = new NetCDFDataSet(path);

            Log.Logger.Debug("Successfully loaded product,\n\t- Name: {0}\n\t- Dimensions array count: {1}\n\t- Variables array count: {2}",
                this.InternalDataSet.Name,
                this.InternalDataSet.Dimensions.Count,
                this.InternalDataSet.Variables.Count);
        }
    }
}
