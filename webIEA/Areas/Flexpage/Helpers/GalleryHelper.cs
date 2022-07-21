using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

using Flexpage.Code.Helpers;


namespace Flexpage.Helpers
{
    public static class GalleryHelper
    {
        public class ThumbProperties
        {
            public string Postfix { get; set; }
            public int Size { get; set; }
        }

        public const string SmallThumbPostfix = "_ltsmall";
        public const string BigThumbPostfix = "_ltbig";

        public const int SmallThumbSize = 512;
        public const int BigThumbSize = 1024;

        public static List<ThumbProperties> ThumbsPrefixes = new List<ThumbProperties>()
        {
            new ThumbProperties()
            {
                Postfix = SmallThumbPostfix,
                Size = SmallThumbSize,
            },
            new ThumbProperties()
            {
                Postfix = BigThumbPostfix,
                Size = BigThumbSize,
            },
        };

        public const string ThumbDirectory = "Thumb";

        public static byte[] GetFileData(HttpServerUtilityBase server, string path, out string fileName, out string contentType, 
            ThumbProperties thumbInfo = null, bool returnPlaceholder = false, bool slider = false)
        {
            string abspath = server.MapPath(path);
            fileName = Path.GetFileName(abspath);

            if (returnPlaceholder)
            {
                abspath = server.MapPath("/Areas/Flexpage/Content/Images/placeholder.png");
            }
            else
            {
                if(thumbInfo == null && slider)
                {
                    thumbInfo = new ThumbProperties() { Postfix = SmallThumbPostfix, Size = SmallThumbSize };
                }

                string shortfilename = Path.GetFileNameWithoutExtension(abspath);

                if (thumbInfo != null)
                {
                    //need to get thumb
                    abspath = Path.Combine(Path.GetDirectoryName(abspath), ThumbDirectory);
                    if (!Directory.Exists(abspath))
                        Directory.CreateDirectory(abspath);

                    abspath = Path.Combine(abspath, fileName);
                    if (!System.IO.File.Exists(abspath))
                    {
                        //generate thumb
                        string sourcepath = server.MapPath(path.Replace(thumbInfo.Postfix, ""));
                        ImageUtitlity.SaveToJpeg(ImageUtitlity.GetReducedImage(System.Drawing.Image.FromFile(sourcepath), thumbInfo.Size, thumbInfo.Size), abspath);
                    }
                }
            }
            contentType = MimeMapping.GetMimeMapping(abspath);
            //prepare result
            return System.IO.File.ReadAllBytes(abspath);
        }
    }
}