using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace MonolithicNetCore.Common
{
    public class ImageHepler
    {
        public static Image ResizeImage(Image image, int width, int height)
        {
            Image newImage = new Bitmap(width, height);

            using (Graphics GFX = Graphics.FromImage((Bitmap)newImage))
            {
                GFX.DrawImage(image, new Rectangle(Point.Empty, new Size(width, height)));
            }

            return newImage;
        }

        public static void Resize(Stream input, Stream output, int width, int height)
        {
            using (var image = Image.FromStream(input))
            using (var bmp = new Bitmap(width, height))
            using (var gr = Graphics.FromImage(bmp))
            {
                gr.CompositingQuality = CompositingQuality.HighSpeed;
                gr.SmoothingMode = SmoothingMode.HighSpeed;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.CompositingMode = CompositingMode.SourceCopy;
                gr.DrawImage(image, 0, 0, width, height);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    bmp.Save(output, ImageFormat.Jpeg);
                }
            }
        }
    }
}
