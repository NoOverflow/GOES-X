using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using GOES_I.Utils;
using Microsoft.Research;
using Microsoft.Research.Science.Data.NetCDF4;

namespace GOES_I
{
    public class Product
    {
        public NetCDFDataSet InternalDataSet { get; set; }


        public Product(string path)
        {
            this.InternalDataSet = new NetCDFDataSet(path);
            
            Console.WriteLine("[Debug - DataSet]");
            Console.WriteLine("    - Name: " + this.InternalDataSet.Name);
            Console.WriteLine("    [Dimensions]");
            foreach (var dimension in this.InternalDataSet.Dimensions)
            {
                Console.WriteLine("        - " + dimension.Name);
            }
            Console.WriteLine("    [Variables]");
            foreach (var variable in this.InternalDataSet.Variables)
            {
                Console.WriteLine("        - " + variable.Name);
            }
            // this.Radiances = (short[,])(this.InternalDataSet["Rad"].GetData());
        }
    }
}
