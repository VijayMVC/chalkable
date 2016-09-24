using System.Collections.Generic;
using System.IO;
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
            return await Connector.Get<Announcement>($"{url}?announcementId={announcementId}&announcementType={(int)announcementType}");
        }

        public async Task<Announcement> UploadAttributeAttachment(int announcementId, AnnouncementType announcementType, int attributeId, string fileName, Stream stream)
        {
            var url = $"/AnnouncementAttribute/UploadAttachment.json";
            return await Connector.Put<Announcement>($"{url}?announcementId={announcementId}&announcementType={(int)announcementType}" +
                                                      $"&assignedAttributeId={attributeId}&filename={fileName}", stream);
        }
        public async Task<Announcement> UploadAnnouncementAttachment(int announcementId, AnnouncementType announcementType, string fileName, Stream stream)
        {
            var url = "/AnnouncementAttachment/UploadAnnouncementAttachment.json";
            return await Connector.Put<Announcement>($"{url}?announcementId={announcementId}&announcementType={(int)announcementType}&filename={fileName}", stream);
        }

        public async Task<bool> UpdateAnnouncementApplicationMeta(int announcementApplicationId, string text, string imageUrl, string description)
        {
            var url = "/Application/UpdateAnnouncementApplicationMeta.json";

            var nvc = HttpUtility.ParseQueryString(string.Empty);
            nvc.Add("announcementApplicationId", announcementApplicationId.ToString());
            nvc.Add("text", text);
            nvc.Add("imageUrl", imageUrl);
            nvc.Add("description", description);

            return await Connector.Post<bool>(url, nvc);
        }

        public async Task<IList<AnnouncementApplicationRecipient>> GetAnnouncementApplicationRecipients(int? studentId)
        {
            var url = "/Application/AnnouncementApplicationRecipients.json";
            return await Connector.Get<IList<AnnouncementApplicationRecipient>>($"{url}?studentId={studentId}");
        }
        public async Task<IList<int>> GetStudentAnnouncementApplicationIds()
        {
            return await Connector.Get<IList<int>>("/Application/AnnouncementApplicationRecipients.json");
        }

        public async Task<bool> UpdateStudentAnnouncementApplicationMeta(int announcementApplicationId, int studentId, string text)
        {
            var url = "/Application/UpdateStudentAnnouncementApplicationMeta.json";

            var nvc = HttpUtility.ParseQueryString(string.Empty);
            nvc.Add("announcementApplicationId", announcementApplicationId.ToString());
            nvc.Add("studentId", studentId.ToString());
            nvc.Add("text", text);

            return await Connector.Post<bool>(url, nvc);
        }

        public async Task<IList<SchoolPerson>> GetAnnouncementRecipients(int announcementId, StudentFilterEnum? studentfilter, int start = 0, int count = int.MaxValue)
        {
            var url = "/Announcement/GetAnnouncementRecipients.json";
            return await Connector.Get<IList<SchoolPerson>>($"{url}?announcementId={announcementId}&studentFilter={(int?)studentfilter}&start={start}&count={count}");
        }
    }
}