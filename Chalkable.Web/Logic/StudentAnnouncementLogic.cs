using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models;

namespace Chalkable.Web.Logic
{
    public class StudentAnnouncementLogic
    {
        public static StudentAnnouncementsViewData ItemGradesList(IServiceLocatorSchool serviceLocator, AnnouncementDetails announcement
            ,IList<AnnouncementAttachmentInfo> announcementAttachmentInfos)
        {
            var gradingItems = announcement.StudentAnnouncements.ToList();
            gradingItems = gradingItems.OrderBy(x => x.Student.LastName).ThenBy(x => x.Student.FirstName).ToList();
            var res = StudentAnnouncementsViewData.Create(announcement, gradingItems, announcementAttachmentInfos, announcement.GradingStyle);
            res.GradingStyleMapper = GradingStyleLogic.GetGradingStyleMapper(serviceLocator);
            return res;
        }
    }
}