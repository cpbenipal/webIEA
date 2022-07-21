using Flexpage.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace Flexpage.Code.Helpers
{
    public static class ImageUtitlity
    {
        public static Image Inscribe(Image image, int size)
        {
            return Inscribe(image, size, size);
        }
        public static Image Inscribe(Image image, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using(Graphics graphics = Graphics.FromImage(result))
            {
                double factor = 1.0 * width / image.Width;
                if(image.Height * factor < height)
                    factor = 1.0 * height / image.Height;
                Size size = new Size((int)(width / factor), (int)(height / factor));
                Point sourceLocation = new Point((image.Width - size.Width) / 2, (image.Height - size.Height) / 2);

                SmoothGraphics(graphics);
                graphics.DrawImage(image, new Rectangle(0, 0, width, height), new Rectangle(sourceLocation, size), GraphicsUnit.Pixel);
            }
            return result;
        }
        public static Image GetReducedImage(Image image, int extremeWidth, int extremeHeight)
        {
            double ratio = (double)image.Width / (double)image.Height;
            Size size = new Size(extremeWidth, (int)(extremeWidth / ratio));
            if(size.Height > extremeHeight)
                size = new Size((int)(extremeHeight * ratio), extremeHeight);
            return new Bitmap(image, size);
        }
        public static Image GetReducedImage(string path, int extremeWidth, int extremeHeight)
        {
            Image image = Image.FromFile(path);
            double ratio = (double)image.Width / (double)image.Height;
            Size size = new Size(extremeWidth, (int)(extremeWidth / ratio));
            if(size.Height > extremeHeight)
                size = new Size((int)(extremeHeight * ratio), extremeHeight);
            return new Bitmap(image, size);
        }

        public static Image GetThumbnail(Image image, int extremeWidth, int extremeHeight)
        {
            return image.GetThumbnailImage(extremeWidth, extremeHeight, () => false, IntPtr.Zero);
        }

        public static string GetThumbnailInBase64(Image image, int extremeWidth, int extremeHeight)
        {
            image = GetThumbnail(image, extremeWidth, extremeHeight);
            byte[] imageBytes = (byte[])(new ImageConverter()).ConvertTo(image, typeof(byte[]));
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }

        public static string GetReducedImageInBase64(string path, int extremeWidth, int extremeHeight, bool isFileSystemPath = true)
        {
            if(isFileSystemPath == false)
            {
                if(path.Contains("~/") == false)
                {
                    if((path[0] == '/') == false)
                    {
                        path = "~/" + path;
                    }
                    else
                    {
                        path = "~" + path;
                    }
                }
                path = System.Web.HttpContext.Current.Server.MapPath(path);
            }
            Image image = Image.FromFile(path);

            double ratio = (double)image.Width / (double)image.Height;
            Size size = new Size(extremeWidth, (int)(extremeWidth / ratio));
            if(size.Height > extremeHeight)
                size = new Size((int)(extremeHeight * ratio), extremeHeight);
            Bitmap newImage = new Bitmap(image, size);
            MemoryStream ms = new MemoryStream();

            newImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] imageBytes = ms.ToArray();
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }
        public static string GetReducedImageInBase64(string path, int extremeWidth, int extremeHeight, bool addWaterMark, string waterMarkText, bool isFileSystemPath = true)
        {
            if(isFileSystemPath == false)
            {
                if(path.Contains("~/") == false)
                {
                    if((path[0] == '/') == false)
                    {
                        path = "~/" + path;
                    }
                    else
                    {
                        path = "~" + path;
                    }
                }
                path = System.Web.HttpContext.Current.Server.MapPath(path);
            }
            Image image = Image.FromFile(path);
            if(addWaterMark == true && string.IsNullOrEmpty(waterMarkText) == false)
            {
                DrawWatermarkText(image, waterMarkText);
            }
            double ratio = (double)image.Width / (double)image.Height;
            Size size = new Size(extremeWidth, (int)(extremeWidth / ratio));
            if(size.Height > extremeHeight)
                size = new Size((int)(extremeHeight * ratio), extremeHeight);
            Bitmap newImage = new Bitmap(image, size);
            MemoryStream ms = new MemoryStream();

            newImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] imageBytes = ms.ToArray();
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }

        public static string GetImageInBase64(string path, bool addWaterMark, string waterMarkText, bool isFileSystemPath = true)
        {
            if(isFileSystemPath == false)
            {
                if(path.Contains("~/") == false)
                {
                    if((path[0] == '/') == false)
                    {
                        path = "~/" + path;
                    }
                    else
                    {
                        path = "~" + path;
                    }
                }
                path = System.Web.HttpContext.Current.Server.MapPath(path);
            }
            Image image = Image.FromFile(path);
            if(addWaterMark == true && string.IsNullOrEmpty(waterMarkText) == false)
            {
                DrawWatermarkText(image, waterMarkText);
            }
            byte[] imageBytes = (byte[])(new ImageConverter()).ConvertTo(image, typeof(byte[]));
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }

        public static string AddWaterToImage(string path, string waterMarkText, string waterMarkImagesFolder, bool isFileSystemPath = true)
        {
            if(Directory.Exists(waterMarkImagesFolder) == false)
            {
                Directory.CreateDirectory(waterMarkImagesFolder);
            }

            if(isFileSystemPath == false)
            {
                if(path.Contains("~/") == false)
                {
                    if((path[0] == '/') == false)
                    {
                        path = "~/" + path;
                    }
                    else
                    {
                        path = "~" + path;
                    }
                }
                path = System.Web.HttpContext.Current.Server.MapPath(path);
            }
            Image image = Image.FromFile(path);
            if(string.IsNullOrEmpty(waterMarkText) == false)
            {
                DrawWatermarkText(image, waterMarkText);
                string watermarkedFilePath = waterMarkImagesFolder + "\\" + Path.GetFileName(path);
                if(File.Exists(watermarkedFilePath) == true)
                {
                    File.Delete(watermarkedFilePath);
                }
                image.Save(waterMarkImagesFolder + "\\" + Path.GetFileName(path));
                watermarkedFilePath = Flexpage.Code.Common.UrlHelper.GetVirtualPath(watermarkedFilePath).Trim(new char['~']);
                return watermarkedFilePath;
            }
            else
            {
                return path;
            }
        }

        public static void DrawWatermarkText(Image image, string text)
        {
            DrawWatermarkText(image, text, "Arial");
        }
        public static void DrawWatermarkText(Image image, string text, string fontName)
        {
            using(Graphics graphics = CreateGraphics(image))
                DrawWatermarkText(graphics, text, fontName);
        }
        public static void DrawWatermarkText(Graphics graphics, string text, string fontName)
        {
            int imageHeight = (int)graphics.VisibleClipBounds.Height;
            int imageWidth = (int)graphics.VisibleClipBounds.Width;
            int maxTextWidth = (int)(imageHeight * 0.4);
            int[] fontSizes = new int[] { 72, 48, 36, 24, 18, 18, 14, 12, 10 };
            Font font = null;
            foreach(int fontSize in fontSizes)
            {
                font = new Font(fontName, fontSize, GraphicsUnit.Pixel);
                if(graphics.MeasureString(text, font).Width <= maxTextWidth)
                    break;
            }
            GraphicsState state = graphics.Save();
            SmoothGraphics(graphics);
            graphics.RotateTransform(-90);
            float padding = font.Size / 2;
            graphics.TranslateTransform(-imageHeight + padding, imageWidth - font.GetHeight() - padding);
            graphics.TextContrast = 12;
            graphics.PageUnit = font.Unit;
            graphics.DrawString(text, font, new SolidBrush(Color.FromArgb(120, Color.Black)), 1, 1);
            graphics.DrawString(text, font, new SolidBrush(Color.FromArgb(120, Color.White)), 0, 0);
            graphics.Restore(state);
        }
        static void SmoothGraphics(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }
        static Graphics CreateGraphics(Image image)
        {
            image = IsIndexedPixelFormat(image.PixelFormat) ? ConvertIndexedBitmapToARGB(image) : image;
            Graphics graphics = Graphics.FromImage(image);
            SmoothGraphics(graphics);
            return graphics;
        }
        static bool IsIndexedPixelFormat(PixelFormat pixelFormat)
        {
            switch(pixelFormat)
            {
                case PixelFormat.Format1bppIndexed:
                    return true;
                case PixelFormat.Format4bppIndexed:
                    return true;
                case PixelFormat.Format8bppIndexed:
                    return true;
                case PixelFormat.Indexed:
                    return true;
                default:
                    return false;
            }
        }
        static Bitmap ConvertIndexedBitmapToARGB(Image image)
        {
            Bitmap result = new Bitmap(image.Width, image.Height);
            using(Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(image, new Rectangle(0, 0, result.Width, result.Height), 0, 0, result.Width, result.Height, GraphicsUnit.Pixel);
                return result;
            }
        }
        public static void SaveToJpeg(Image image, Stream output)
        {
            image.Save(output, ImageFormat.Jpeg);
        }
        public static void SaveToJpeg(Image image, string fileName)
        {
            image.Save(fileName, ImageFormat.Jpeg);
        }
    }
}