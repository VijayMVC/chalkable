using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Tests.Services.Master
{
    public class PictureServiceTest : MasterServiceTestBase
    {
        public static void LoadImage(string path, out Image image, out byte[] imageContent)
        {
            image = Image.FromFile(path);
            var stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);
            imageContent = stream.GetBuffer();
            stream.Close();
        }
    }
}
