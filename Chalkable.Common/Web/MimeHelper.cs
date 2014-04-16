using System.Collections.Generic;
using System.IO;

namespace Chalkable.Common.Web
{
    public static class MimeHelper
    {
        public static string GetContenttypeByExt(string ext)
        {
            if (attachmenTypes.ContainsKey(ext))
                return attachmenTypes[ext].Second;
            return "application/octetstream";
        }


        public enum AttachmenType
        {
            Document = 0,
            Picture = 1,
            Unknown = 2
        }

        private static Dictionary<string, Pair<AttachmenType, string>> attachmenTypes = new Dictionary<string, Pair<AttachmenType, string>>
                                                                              {
                                                                                  {"pdf", new Pair<AttachmenType, string>(AttachmenType.Document, "application/pdf")},
                                                                                  {"doc", new Pair<AttachmenType, string>(AttachmenType.Document, "application/doc")},
                                                                                  {"docx", new Pair<AttachmenType, string>(AttachmenType.Document, "application/vnmimes.openxmlformats-officedocument.wordprocessingml.document")},

                                                                                  {"jpg", new Pair<AttachmenType, string>(AttachmenType.Picture, "image/jpg")},
                                                                                  {"gif", new Pair<AttachmenType, string>(AttachmenType.Picture, "image/gif")},
                                                                                  {"jpeg", new Pair<AttachmenType, string>(AttachmenType.Picture, "image/jpeg")},
                                                                                  {"png", new Pair<AttachmenType, string>(AttachmenType.Picture, "image/png")},
                                                                                  {"tif", new Pair<AttachmenType, string>(AttachmenType.Picture, "image/tif")},
                                                                                  {"bmp", new Pair<AttachmenType, string>(AttachmenType.Picture, "image/bmp")},
                                                                                  {"tiff", new Pair<AttachmenType, string>(AttachmenType.Picture, "image/tiff")},

                                                                                  {"ogg", new Pair<AttachmenType, string>(AttachmenType.Unknown, "application/ogg")},
                                                                                  {"wma", new Pair<AttachmenType, string>(AttachmenType.Unknown, "audio/x-ms-wma")},
                                                                                  {"mp3", new Pair<AttachmenType, string>(AttachmenType.Unknown, "audio/mpeg")},
                                                                                  {"wav", new Pair<AttachmenType, string>(AttachmenType.Unknown, "audio/x-wav")},
                                                                                  {"wmv", new Pair<AttachmenType, string>(AttachmenType.Unknown, "audio/x-ms-wmv")},
                                                                                  {"swf", new Pair<AttachmenType, string>(AttachmenType.Unknown, "application/x-shockwave-flash")},
                                                                                  
                                                                                  {"avi", new Pair<AttachmenType, string>(AttachmenType.Unknown, "video/avi")},
                                                                                  {"mp4", new Pair<AttachmenType, string>(AttachmenType.Unknown, "video/mp4")},
                                                                                  {"mpeg", new Pair<AttachmenType, string>(AttachmenType.Unknown, "video/mpeg")},
                                                                                  {"mpg", new Pair<AttachmenType, string>(AttachmenType.Unknown, "video/mpeg")},
                                                                                  {"qt", new Pair<AttachmenType, string>(AttachmenType.Unknown, "video/quicktime")},

                                                                                  {"zip", new Pair<AttachmenType, string>(AttachmenType.Unknown, "application/zip")},
                                                                                  
                                                                                  {"xlsx", new Pair<AttachmenType, string>(AttachmenType.Unknown, "application/vnmimes.openxmlformats-officedocument.spreadsheetml.sheet")},
                                                                                  {"xls", new Pair<AttachmenType, string>(AttachmenType.Unknown, "application/vnmimes.ms-excel")},
                                                                                  {"csv", new Pair<AttachmenType, string>(AttachmenType.Unknown, "text/csv")},
                                                                                  {"xml", new Pair<AttachmenType, string>(AttachmenType.Unknown, "text/xml")},
                                                                                  {"txt", new Pair<AttachmenType, string>(AttachmenType.Unknown, "text/plain")},
                                                                                  
                                                                                  {"ppt", new Pair<AttachmenType, string>(AttachmenType.Unknown, "application/vnmimes.ms-powerpoint")},
                                                                                  {"pptx", new Pair<AttachmenType, string>(AttachmenType.Unknown, "application/vnmimes.openxmlformats-officedocument.wordprocessingml.document")},
                                                                              };

        public static string GetContentTypeByName(string fileName)
        {
            var ext = GetExtension(fileName);
            if (attachmenTypes.ContainsKey(ext.ToLower()))
            {
                return attachmenTypes[ext.ToLower()].Second;
            }
            return "attachment/" + ext;
        }

        public static AttachmenType GetTypeByName(string fileName)
        {
            var ext = GetExtension(fileName);
            return attachmenTypes.ContainsKey(ext.ToLower()) ? attachmenTypes[ext.ToLower()].First : AttachmenType.Unknown;
        }
        private static string GetExtension(string fileName)
        {
            return (Path.GetExtension(fileName) ?? string.Empty).Replace(".", "").Trim(' ');
        }
    }
}