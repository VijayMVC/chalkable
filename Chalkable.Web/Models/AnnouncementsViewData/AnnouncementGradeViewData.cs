using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Mapping;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AnnouncementGradeViewData : AnnouncementViewData
    {
        public decimal? Grade { get; set; }
        public string Comment { get; set; }

        protected AnnouncementGradeViewData(AnnouncementComplex announcement, IList<StudentAnnouncement> studentAnnouncements)
            : base(announcement)
        {
            PrepareGradingInfo(this, studentAnnouncements);
        }

        protected AnnouncementGradeViewData(AnnouncementComplex announcement)
            : base(announcement)
        {
        }

        public static AnnouncementGradeViewData Create(AnnouncementComplex announcement, IList<StudentAnnouncement> studentAnnouncements)
        {
           return new AnnouncementGradeViewData(announcement, studentAnnouncements);
        }

        private static void PrepareGradingInfo(AnnouncementGradeViewData res,  IList<StudentAnnouncement> studentAnnouncements)
        {
            if (studentAnnouncements != null && studentAnnouncements.Count > 0)
            {
                if (studentAnnouncements.Count == 1)
                {
                    var studentAnnouncement = studentAnnouncements.First();
                    res.Grade = studentAnnouncement.NumericScore;
                    res.Comment = studentAnnouncement.Comment;
                }
            }
        }
    }
}