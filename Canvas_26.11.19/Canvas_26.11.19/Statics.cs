using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Canvas_26._11._19
{
    static class Statics
    {

        public static Color fieldsBackColor;
        public static Color penColor = Color.Aqua;
        public static Color initiatePenColor;

        public static void drawBorder(this Control control, int borderWidth, Color bordercolor)
        {
            Bitmap bitmap = new Bitmap(control.Width, control.Height);
            Graphics graphicsObj = Graphics.FromImage(bitmap);

            Pen myPen = new Pen(bordercolor, borderWidth);
            graphicsObj.DrawRectangle(myPen, 0, 0, control.Width - 1 , control.Height - 1);

            control.BackgroundImage = bitmap;
            graphicsObj.Dispose();
        }
        public static void drawBorder(this Image image, int borderWidth, Color bordercolor)
        {
            Bitmap bitmap = new Bitmap(image.Width, image.Height);
            Graphics graphicsObj = Graphics.FromImage(bitmap);

            Pen myPen = new Pen(bordercolor, borderWidth);
            graphicsObj.DrawRectangle(myPen, 0, 0, image.Width - 1, image.Height - 1);

            image = bitmap;
            graphicsObj.Dispose();
        }








        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(this Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }



        public static Bitmap BlockyResizeImage(this Bitmap image, int width, int height)
        {
            int widthFactor = width / image.Width;
            int heightfactor = height / image.Height;

            Color[,] colorArray = new Color[image.Width, image.Height];

            for(int i = 0; i < image.Width; i++)
            {
                for(int j = 0; j < image.Height; j++)
                {
                    colorArray[i, j] = image.GetPixel(i, j);
                }
            }

            Color[,] resizedColorArray = new Color[image.Width * widthFactor, image.Height * heightfactor];

            Bitmap resizedImage = new Bitmap(width, height);

            for (int i = 0; i < resizedColorArray.GetLength(0); i++)
            {
                for (int j = 0; j < resizedColorArray.GetLength(1); j++)
                {
                    resizedColorArray[i, j] = colorArray[i / widthFactor, j / heightfactor];
                    resizedImage.SetPixel(i, j, resizedColorArray[i, j]);

                }
            }
            

            using (Graphics graphicsPbj = Graphics.FromImage(resizedImage))
            {
                graphicsPbj.DrawImage(resizedImage, 0, 0);
            }

            //resizedImage

            resizedImage.drawBorder(1, Color.Black);

            return resizedImage;


        }
    }
}
