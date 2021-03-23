using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Magician.Common.Util
{
    public class ImageUtil
    {
        private static readonly float[][] ColorMatrix =
        {
            new[] {0.299f, 0.299f, 0.299f, 0, 0},
            new[] {0.587f, 0.587f, 0.587f, 0, 0},
            new[] {0.114f, 0.114f, 0.114f, 0, 0},
            new[] {0f, 0, 0, 1, 0},
            new[] {0f, 0, 0, 0, 1}
        };

        public static Image ConvertBuffer2Image(byte[] buffer)
        {
            if (buffer.Length == 0) return null;
            Image img;
            using (var ms = new MemoryStream(buffer))
            {
                img = Image.FromStream(ms);
            }

            return img;
        }


        /// <summary>
        ///     将图像转化为灰度图像。
        /// </summary>
        public static Bitmap ConvertToGrey(Image origin)
        {
            var newBitmap = new Bitmap(origin);
            using (var g = Graphics.FromImage(newBitmap))
            {
                var ia = new ImageAttributes();
                var cm = new ColorMatrix(ColorMatrix);
                ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                g.DrawImage(newBitmap, new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), 0, 0, newBitmap.Width,
                    newBitmap.Height, GraphicsUnit.Pixel, ia);
            }

            return newBitmap;
        }

        /// <summary>
        ///     从字节数组中加载图片。
        /// </summary>
        public static Image Convert(byte[] buff)
        {
            var ms = new MemoryStream(buff);
            var img = Image.FromStream(ms);
            ms.Close();
            return img;
        }

        public static Image ConvertToJpg(Image img)
        {
            var ms = new MemoryStream();
            img.Save(ms, ImageFormat.Jpeg);
            var img2 = Image.FromStream(ms);
            ms.Close();
            return img2;
        }

        /// <summary>
        ///     深度复制图片。
        /// </summary>
        public static Image CopyImageDeeply(Image img)
        {
            var bmp2 = new Bitmap(img.Width, img.Height, img.PixelFormat);
            using (var g = Graphics.FromImage(bmp2))
            {
                g.DrawImage(img, 0, 0, img.Width,
                    img.Height); //不能为 g.DrawImage(img, new Point(0, 0)); 否则因为dpi的问题，可能只绘制部分图像
            }

            return bmp2;
        }

        /// <summary>
        ///     将图像使用JPEG格式存储。
        /// </summary>
        public static byte[] Convert(Image img)
        {
            //需要将图片先复制一份
            var bmp2 = CopyImageDeeply(img);

            var ms = new MemoryStream();
            bmp2.Save(ms, ImageFormat.Jpeg);
            //bmp2.Save(ms, ImageFormat.Bmp);
            var buff = ms.ToArray();
            ms.Close();

            bmp2.Dispose(); //释放bmp文件资源
            return buff;
        }

        public static void Save(Image img, string path, ImageFormat format)
        {
            if (img == null || path == null) return;

            //需要将图片先复制一份
            var bmp2 = CopyImageDeeply(img);
            bmp2.Save(path, format);
        }

        public static bool IsGif(Image img)
        {
            var guids = img.FrameDimensionsList;
            var fd = new FrameDimension(guids[0]);
            return img.GetFrameCount(fd) > 1;
        }

        public static Icon ConvertToIcon(Image img, int iconLength)
        {
            using (var bm = new Bitmap(img, new Size(iconLength, iconLength)))
            {
                return Icon.FromHandle(bm.GetHicon());
            }
        }

        public static Bitmap ConstructRgb24Bitmap(byte[] coreData, int width, int height)
        {
            var bm = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            var bitmapData = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb); //不包括头的数据
            Marshal.Copy(coreData, 0, bitmapData.Scan0, coreData.Length); //(bitmapData.Scan0, data, 0, data.Length);
            bm.UnlockBits(bitmapData);
            return bm;
        }

        public static byte[] GetRgb24CoreData(Bitmap bm)
        {
            var imageBuff = new byte[bm.Width * bm.Height * 3];
            var data = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb); //不包括头的数据
            Marshal.Copy(data.Scan0, imageBuff, 0, imageBuff.Length);
            bm.UnlockBits(data);
            return imageBuff;
        }

        /// <summary>
        ///     截取RGB24图像的一部分（从左上角开始）。
        /// </summary>
        /// <param name="origin">原始位图的核心数据</param>
        /// <param name="originSize">位图大小</param>
        /// <param name="newSize">要截取的大小</param>
        /// <returns>被截的部分图像的RGB24数据</returns>
        public static byte[] ReviseRgb24Data(byte[] origin, Size originSize, Size newSize)
        {
            var oldBm = ConstructRgb24Bitmap(origin, originSize.Width, originSize.Height);

            var newBitmap = new Bitmap(newSize.Width, newSize.Height);
            using (var g = Graphics.FromImage(newBitmap))
            {
                g.DrawImage(oldBm, 0, 0, new RectangleF(0, 0, newSize.Width, newSize.Height),
                    GraphicsUnit.Pixel); //不能为 g.DrawImage(img, new Point(0, 0)); 否则因为dpi的问题，可能只绘制部分图像
            }

            var imageBuff = GetRgb24CoreData(newBitmap);
            return imageBuff;
        }

        /// <summary>
        ///     抠取原图的一部分形成一个新的位图。
        /// </summary>
        /// <param name="origin">原始图像</param>
        /// <param name="rect">要抠取的区域</param>
        public static Bitmap GetPart(Image origin, Rectangle rect)
        {
            var newImg = new Bitmap(rect.Width, rect.Height);
            using (var g = Graphics.FromImage(newImg))
            {
                g.DrawImage(origin, 0, 0, rect,
                    GraphicsUnit.Pixel); //不能为 g.DrawImage(origin, rect);  因为dpi的问题，可能只绘制部分图像 。2016.11.14
            }

            return newImg;
        }

        /// <summary>
        ///     将图像的长和宽裁剪为N的整数倍。
        /// </summary>
        /// <param name="origin">原始图像</param>
        /// <param name="number">整数倍的基数</param>
        public static Bitmap RoundSizeByNumber(Bitmap origin, int number)
        {
            if (origin.Width % number == 0 && origin.Height % number == 0) return origin;

            var newWidth = origin.Width / number * number;
            var newHeight = origin.Height / number * number;

            var newImg = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(newImg))
            {
                g.DrawImage(origin, new Rectangle(0, 0, newWidth, newHeight));
            }

            return newImg;
        }

        /// <summary>
        ///     缩放图片。
        /// </summary>
        public static Bitmap Zoom(Image origin, float zoomCoef)
        {
            var newImg = new Bitmap((int) (origin.Width * zoomCoef), (int) (origin.Height * zoomCoef));
            using (var g = Graphics.FromImage(newImg))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.DrawImage(origin, new Rectangle(0, 0, newImg.Width, newImg.Height));
            }

            return newImg;
        }

        /// <summary>
        ///     缩放图片。
        /// </summary>
        public static Bitmap Zoom(Image origin, Size newSize)
        {
            var newImg = new Bitmap(newSize.Width, newSize.Height);
            using (var g = Graphics.FromImage(newImg))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.DrawImage(origin, new Rectangle(0, 0, newImg.Width, newImg.Height));
            }

            return newImg;
        }

        /// <summary>
        ///     旋转图片。
        /// </summary>
        public static Bitmap Rotate(Image src, int angle)
        {
            angle = angle % 360;

            //弧度转换
            var radian = angle * Math.PI / 180.0;
            var cos = Math.Cos(radian);
            var sin = Math.Sin(radian);

            //原图的宽和高
            var w = src.Width;
            var h = src.Height;
            var dstW = (int) Math.Max(Math.Abs(w * cos - h * sin), Math.Abs(w * cos + h * sin));
            var dstH = (int) Math.Max(Math.Abs(w * sin - h * cos), Math.Abs(w * sin + h * cos));

            //目标位图
            var dsImage = new Bitmap(dstW, dstH);
            using (var g = Graphics.FromImage(dsImage))
            {
                g.InterpolationMode = InterpolationMode.Bilinear;
                g.SmoothingMode = SmoothingMode.HighQuality;

                //计算偏移量
                var offset = new Point((dstW - w) / 2, (dstH - h) / 2);
                //构造图像显示区域：让图像的中心与窗口的中心点一致
                var rect = new Rectangle(offset.X, offset.Y, w, h);
                var center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
                g.TranslateTransform(center.X, center.Y);
                g.RotateTransform(360 - angle);

                //恢复图像在水平和垂直方向的平移
                g.TranslateTransform(-center.X, -center.Y);
                g.DrawImage(src, rect);

                //重至绘图的所有变换
                g.ResetTransform();
                g.Save();
            }

            return dsImage;
        }
    }
}