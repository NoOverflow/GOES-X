using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Linq;
using Serilog.Core;
using Serilog;
using System.Diagnostics;

namespace GOES_I.Utils
{
    public static class ImageUtils
    {
        public static Image CreateRGBImage(double[,] R, double[,] G, double[,] B, double gamma = 2.2, double? transparencyKey = null)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();
            Log.Logger.Debug("Creating RGB image, gamma={0} transparency_key={1}", gamma, transparencyKey);
            double rMin = R.Cast<double>().Min();
            rMin = Math.Clamp((double)rMin, (double)0, (double)255);
            double rMax = R.Cast<double>().Max();
            rMax = Math.Clamp((double)rMax, (double)0, (double)255);
            double rRange = (double)(rMax - rMin);
            
            double gMin = G.Cast<double>().Min();
            gMin = Math.Clamp((double)gMin, (double)0, (double)255);
            double gMax = G.Cast<double>().Max();
            gMax = Math.Clamp((double)gMax, (double)0, (double)255);
            double gRange = (double)(gMax - gMin);

            double bMin = B.Cast<double>().Min();
            bMin = Math.Clamp((double)bMin, (double)0, (double)255);
            double bMax = B.Cast<double>().Max();
            bMax = Math.Clamp((double)bMax, (double)0, (double)255);
            double bRange = (double)(bMax - bMin);

            byte rV;
            double rsV;

            byte gV;
            double gsV;

            byte bV;
            double bsV;

            Bitmap bm = new Bitmap(R.GetLength(0), R.GetLength(1));
            BitmapData bd = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* ptr = (byte*)bd.Scan0;

                for (int j = 0; j < bd.Height; j++)
                {
                    for (int i = 0; i < bd.Width; i++)
                    {
                        rsV = R[i, bd.Height - 1 - j];
                        gsV = G[i, bd.Height - 1 - j];
                        bsV = B[i, bd.Height - 1 - j];

                        rsV = (double)Math.Pow(rsV, 1 / gamma);
                        gsV = (double)Math.Pow(gsV, 1 / gamma);
                        bsV = (double)Math.Pow(bsV, 1 / gamma);

                        rsV = (double)Math.Clamp((double)rsV, (double)0, (double)255);
                        gsV = (double)Math.Clamp((double)gsV, (double)0, (double)255);
                        bsV = (double)Math.Clamp((double)bsV, (double)0, (double)255);

                        rV = (byte)(255 * (rsV - rMin) / rRange);
                        gV = (byte)(255 * (gsV - gMin) / gRange);
                        bV = (byte)(255 * (bsV - bMin) / bRange);

                        ptr[0] = (byte)(bV);
                        ptr[1] = (byte)(gV);
                        ptr[2] = (byte)(rV);
                        ptr[3] = (byte)((transparencyKey == null) ? 255 : (rV == transparencyKey ? 0 : 255));
                        ptr += 4;
                    }
                    ptr += (bd.Stride - (bd.Width * 4));
                }
            }
            bm.UnlockBits(bd);
            sw.Stop();
            Log.Logger.Debug("RGB image built in {0}ms", sw.ElapsedMilliseconds);
            return bm;
        }

        public static Image CreateImage(double[,] R, double gamma = 2.2, Color? baseColor = null, double? transparencyKey = null)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();
            Log.Logger.Debug("Creating RGB image, gamma={0} transparency_key={1}", gamma, transparencyKey);

            double rMin = R.Cast<double>().Min();
            rMin = Math.Clamp((double)rMin, (double)0, (double)255);
            double rMax = R.Cast<double>().Max();
            rMax = Math.Clamp((double)rMax, (double)0, (double)255);
            double rRange = (double)(rMax - rMin);


            byte rV;
            double rsV;

            Bitmap bm = new Bitmap(R.GetLength(0), R.GetLength(1));
            BitmapData bd = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* ptr = (byte*)bd.Scan0;

                for (int j = 0; j < bd.Height; j++)
                {
                    for (int i = 0; i < bd.Width; i++)
                    {
                        rsV = R[i, bd.Height - 1 - j];

                        rsV = (double)Math.Pow(rsV, 1 / gamma);

                        rsV = (double)Math.Clamp((double)rsV, (double)0, (double)255);

                        rV = (byte)(255 * (rsV - rMin) / rRange);

                        ptr[0] = (byte)(rV);
                        ptr[1] = (byte)(rV);
                        ptr[2] = (byte)(rV);
                        ptr[3] = (byte)((transparencyKey == null) ? 255 : (rV == transparencyKey ? 0 : 255));
                        ptr += 4;
                    }
                    ptr += (bd.Stride - (bd.Width * 4));
                }
            }
            bm.UnlockBits(bd);
            sw.Stop();
            Log.Logger.Debug("Gscale image built in {0}ms", sw.ElapsedMilliseconds);
            return bm;
        }
    }
}
