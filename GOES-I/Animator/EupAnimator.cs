using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;
using GOES_I.EndUserProducts;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace GOES_I.Animator
{
    public class EupAnimator
    {
        public static ImageData LoadImageData(string path)
        {
            using (Image<Rgba32> image = (Image<Rgba32>)Image.Load(path))
            {
                byte[] pixelBytes = new byte[image.Width * image.Height * Unsafe.SizeOf<Rgba32>()];

                image.CopyPixelDataTo(pixelBytes);
                return ImageData.FromArray(pixelBytes, ImagePixelFormat.Rgba32, image.Width, image.Height);
            }            
        }

        public static string AnimateEup(string productName, string productIndex, DateTime initTimestamp, TimeSpan length)
        {
            var settings = new VideoEncoderSettings(width: 5424, height: 5424, framerate: 1, codec: VideoCodec.H264);
            string tempPath = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".mp4";
            DateTime endTimestamp = initTimestamp - length;
            IEndUserProduct eup = EndUserProducts.EndUserProducts.FromName(productName);

            settings.EncoderPreset = EncoderPreset.Medium;
            settings.CRF = 17;
            using (var file = MediaBuilder.CreateContainer(tempPath).WithVideo(settings).Create())
            {
                // TODO: Change increment
                for (; initTimestamp > endTimestamp; initTimestamp = initTimestamp.AddHours(-1))
                {
                    string imgPath = (string)eup.Get(QueryService.GetCachePath(initTimestamp))[productIndex];

                    if (!File.Exists(imgPath)) continue;
                    ImageData frame = LoadImageData(imgPath);

                    file.Video.AddFrame(frame);
                    Log.Logger.Debug("Added frame.");
                }
            }
            return tempPath;
        }
    }
}
