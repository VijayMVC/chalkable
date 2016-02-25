using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Chalkable.API.Enums;
using Chalkable.API.Models;

namespace Chalkable.API.Endpoints
{
    public class AnnouncementEndpoint : Base
    {
        public AnnouncementEndpoint(IConnector connector) : base(connector)
        {
        }

        public async Task<AnnouncementApplication> GetAnnouncementApplicationById(int announcementApplicationId)
        {
            var url = "/Application/GetAnnouncementApplication.json";
            return await Connector.Get<AnnouncementApplication>($"{url}?announcementApplicationId={announcementApplicationId}");
        }

        public async Task<Announcement> GetRead(int announcementId, AnnouncementType announcementType)
        {
            var url = $"/{announcementType}/Read.json";
            return await Connector.Get<Announcement>($"{url}?announcementId={announcementId}&announcementType={announcementType}");
        }

        public async Task<Announcement> UploadAttributeAttachment(int announcementId, int announcementType, int attributeId, string fileName, Stream stream)
        {
            var url = $"/AnnouncementAttribute/UploadAttachment.json";
            return await Connector.Put<Announcement>($"{url}?announcementId={announcementId}&announcementType={announcementType}" +
                                                      $"&assignedAttributeId={attributeId}&filename={fileName}", stream);
        }
        public async Task<Announcement> UploadAnnouncementAttachment(int announcementId, int announcementType, string fileName, Stream stream)
        {
            var url = $"/AnnouncementAttachment/UploadAnnouncementAttachment.json";
            return await Connector.Put<Announcement>($"{url}?announcementId={announcementId}&announcementType={announcementType}&filename={fileName}", stream);
        }

        public async Task<bool> UpdateAnnouncementApplicationMeta(int announcementApplicationId, AnnouncementType announcementType, string text, string imageUrl)
        {
            var url = "/Application/UpdateAnnouncementApplicationMeta.json";

            var nvc = HttpUtility.ParseQueryString(string.Empty);
            nvc.Add("announcementApplicationId", announcementApplicationId.ToString());
            nvc.Add("announcementType", ((int)announcementType).ToString());
            nvc.Add("text", text);
            nvc.Add("imageUrl", imageUrl);

            return await Connector.Post<bool>(url, nvc);
        }
    }
}