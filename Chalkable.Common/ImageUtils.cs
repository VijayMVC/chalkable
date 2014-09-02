using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Chalkable.Common
{
  public class ImageUtils
  {
    public static Byte[] Scale(Byte[] imgSource, int destWidth, int destHeight, bool notSmaller = false)
    {      
      var imgSourceStream = new MemoryStream(imgSource);
      var source = new Bitmap(imgSourceStream);
      var sourceWidth = source.Width;
      var sourceHeight = source.Height;

      var widthScale = (double)sourceWidth / destWidth;
      var heightScale = (double)sourceHeight / destHeight;
      var scale = widthScale > heightScale ? (notSmaller ? heightScale : widthScale) : (notSmaller ? widthScale : heightScale);

      var width = (int)(sourceWidth / scale + 0.5);
      var height = (int)(sourceHeight / scale + 0.5);

      var bmPhoto = new Bitmap(width, height, PixelFormat.Format32bppArgb);
      bmPhoto.SetResolution(source.HorizontalResolution, source.VerticalResolution);

      var grPhoto = Graphics.FromImage(bmPhoto);
      grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

      grPhoto.DrawImage(source, new Rectangle(0, 0, width, height),
          new Rectangle(0, 0, sourceWidth, sourceHeight),
          GraphicsUnit.Pixel);

      grPhoto.Dispose();
     
      var imgDestStream = new MemoryStream();      
      bmPhoto.Save(imgDestStream, ImageFormat.Png);
      var result = imgDestStream.ToArray();

      imgSourceStream.Dispose();
      imgDestStream.Dispose();
      source.Dispose();
      bmPhoto.Dispose();

      return result;
    }


      public static bool IsValidImage(Stream stream)
      {
          var isImg = false;
          try
          {
              using (var img = Image.FromStream(stream))
              {
                  isImg = Equals(img.RawFormat, ImageFormat.Jpeg) || Equals(img.RawFormat, ImageFormat.Png) ||
                          Equals(img.RawFormat, ImageFormat.Bmp);
              }
          }
          catch (Exception)
          {
              isImg = false;
          }
          return isImg;
      }
  }
}
