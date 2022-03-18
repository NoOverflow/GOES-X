using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Linq;
using Serilog.Core;
using Serilog;
using System.Diagnostics;
using SixLabors.ImageSharp.PixelFormats;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Processing;

namespace GOES_I.Utils
{
    public static class ImageUtils
    {
        public static SixLabors.ImageSharp.Image CreateRGBImage(double[,] R, double[,] G, double[,] B, double gamma = 2.2, double? transparencyKey = null)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();
            Log.Logger.Debug("Creating RGB image, gamma={0} transparency_key={1}", gamma, transparencyKey);

            double rMin = R.Cast<double>().Min();
            double rMax = R.Cast<double>().Max();
            double rRange = (double)(rMax - rMin);
            
            double gMin = G.Cast<double>().Min();
            double gMax = G.Cast<double>().Max();
            double gRange = (double)(gMax - gMin);

            double bMin = B.Cast<double>().Min();
            double bMax = B.Cast<double>().Max();
            double bRange = (double)(bMax - bMin);

            byte rV;
            double rsV;

            byte gV;
            double gsV;

            byte bV;
            double bsV;
            
            int width = R.GetLength(0);
            int height = R.GetLength(1);

            byte[] pixelBytes = new byte[R.GetLength(0) * R.GetLength(1) * Unsafe.SizeOf<Rgba32>()];
            int ptr = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    rsV = R[x, height - 1 - y];
                    gsV = G[x, height - 1 - y];
                    bsV = B[x, height - 1 - y];

                    rsV = (double)Math.Pow(rsV, 1 / gamma);
                    gsV = (double)Math.Pow(gsV, 1 / gamma);
                    bsV = (double)Math.Pow(bsV, 1 / gamma);

                    rV = (byte)(255 * (rsV - rMin) / rRange);
                    gV = (byte)(255 * (gsV - gMin) / gRange);
                    bV = (byte)(255 * (bsV - bMin) / bRange);

                    pixelBytes[ptr] = bV;
                    pixelBytes[ptr + 1] = gV;
                    pixelBytes[ptr + 2] = rV;
                    pixelBytes[ptr + 3] = (byte)((transparencyKey == null) ? 255 : (rV == transparencyKey ? 0 : 255));
                    ptr += 4;
                }
            }

            SixLabors.ImageSharp.Image img = SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(pixelBytes, width, height);

            img.Mutate(c => c.Rotate(90));
            sw.Stop();
            Log.Logger.Debug("RGB image built in {0}ms", sw.ElapsedMilliseconds);
            return img;
        }

        public static SixLabors.ImageSharp.Image CreateImage(double[,] R, double gamma = 2.2, Color? baseColor = null, double? transparencyKey = null)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();
            Log.Logger.Debug("Creating grayscale image, gamma={0} transparency_key={1}", gamma, transparencyKey);

            double rMin = R.Cast<double>().Min();
            double rMax = R.Cast<double>().Max();
            double rRange = (double)(rMax - rMin);

            byte rV;
            double rsV;

            int width = R.GetLength(0);
            int height = R.GetLength(1);

            byte[] pixelBytes = new byte[R.GetLength(0) * R.GetLength(1) * Unsafe.SizeOf<Rgba32>()];
            int ptr = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    rsV = R[x, height - 1 - y];
                    if (rsV == 0)
                    {
                        pixelBytes[ptr + 3] = 255;
                    }
                    else
                    {
                        rsV = (double)Math.Pow(rsV, 1 / gamma);
                        rV = (byte)(255 * (rsV - rMin) / rRange);

                        pixelBytes[ptr] = rV;
                        pixelBytes[ptr + 1] = rV;
                        pixelBytes[ptr + 2] = rV;
                        pixelBytes[ptr + 3] = (byte)((transparencyKey == null) ? 255 : (rV == transparencyKey ? 0 : 255));
                    }
                    ptr += 4;
                }
            }

            SixLabors.ImageSharp.Image img = SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(pixelBytes, width, height);

            img.Mutate(c => c.Rotate(90));
            sw.Stop();
            Log.Logger.Debug("Gscale image built in {0}ms", sw.ElapsedMilliseconds);
            return img;
        }
    }
}
