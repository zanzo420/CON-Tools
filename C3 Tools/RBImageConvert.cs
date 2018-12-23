using System;
using System.IO;
using Pfim;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace C3Tools
{
    public class RBImageConvert
    {
        private RBImageConvert() {}

        private static byte[] ReadGameImageAsDDS(string path) {
            if (Path.GetExtension(path) == "dds")
            {
                return File.ReadAllBytes(path);
            }

            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                return new RBGameImage(stream, path.Contains("_xbox")).ToDDSBytes();
            }
        }

        private static System.Drawing.Bitmap DDSToImage(byte[] ddsBytes)
        {
            Dds dds = Dds.Create(ddsBytes, new PfimConfig());

            using (MemoryStream ms = new MemoryStream())
            {
                switch (dds.Format)
                {
                    case ImageFormat.Rgb24:
                        Image.LoadPixelData<Bgr24>(dds.Data, dds.Width, dds.Height).SaveAsPng(ms);
                        break;
                    case ImageFormat.Rgba32:
                        Image.LoadPixelData<Bgra32>(dds.Data, dds.Width, dds.Height).SaveAsPng(ms);
                        break;
                    default:
                        throw new Exception("DDS Pixel format not handled");
                }

                ms.Seek(0, SeekOrigin.Begin);

                return new System.Drawing.Bitmap(ms);
            }
        }

        public static System.Drawing.Bitmap GameImageFileToBitmap(string path)
        {
            return DDSToImage(ReadGameImageAsDDS(path));
        }
    }
}