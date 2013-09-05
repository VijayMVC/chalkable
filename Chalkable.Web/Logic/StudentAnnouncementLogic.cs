using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
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
            gradingItems = gradingItems.OrderBy(x => x.Person.LastName).ThenBy(x => x.Person.FirstName).ToList();
            if (!announcement.FinalGradeStatusTyped.HasValue)
                throw new ChalkableException("finalGrade not exists");
            var res = StudentAnnouncementsViewData.Create(announcement, gradingItems, announcementAttachmentInfos,
                                                         announcement.FinalGradeStatusTyped.Value, announcement.GradingStyle);
            res.GradingStyleMapper = GradingStyleLogic.GetGradingStyleMapper(serviceLocator);
            return res;
        }
    }
}