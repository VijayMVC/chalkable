using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Chalkable.Api.SampleApp.Controllers
{
    public class DocumentController : BaseSampleAppController
    {
        [HttpPost]
        public async Task<ActionResult> Upload(int announcementId, int announcementType, int? attributeId, string fileName)
        {
            try
            {
                byte[] bin;
                string name;
                if (!GetFileFromRequest(out bin, out name))
                {
                    return Json(new Exception("File required"));
                }
                using (var fileStream = new MemoryStream())
                {
                    fileStream.Write(bin, 0, bin.Length);
                    fileStream.Seek(0, SeekOrigin.Begin);
                    if (attributeId.HasValue)
                        await Connector.Announcement.UploadAttributeAttachment(announcementId, announcementType, attributeId.Value, fileName, fileStream);
                    else
                        await Connector.Announcement.UploadAnnouncementAttachment(announcementId, announcementType, fileName, fileStream);
                }
            }
            catch (Exception e)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = e.ToString() }, JsonRequestBehavior.AllowGet);

                throw;
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index(int announcementId, int announcementType, int? attributeId)
        {
            ViewBag.AnnouncementId = announcementId;
            ViewBag.AnnouncementType = announcementType;
            ViewBag.AttributeId = attributeId;
            return View("Index");
        }


        protected bool GetFileFromRequest(out byte[] bin, out string name, bool onlyImages = false)
        {
            if (Request.Files.Count == 1 && !string.IsNullOrEmpty(Request.Files[0].FileName))
            {
                HttpPostedFileBase hpf = Request.Files[0];
                bin = new byte[hpf.InputStream.Length];
                hpf.InputStream.Read(bin, 0, (int)hpf.InputStream.Length);
                name = hpf.FileName;
                return true;
            }
            bin = null;
            name = null;
            return false;
        }
    }
}