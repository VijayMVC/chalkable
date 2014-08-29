using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Mapping;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AnnouncementGradeViewData : AnnouncementViewData
    {
        protected AnnouncementGradeViewData(AnnouncementComplex announcement, IList<StudentAnnouncement> studentAnnouncements, IGradingStyleMapper mapper, bool? wasAnnouncementTypeGraded = null)
            : base(announcement, wasAnnouncementTypeGraded)
        {
            PrepareGradingInfo(this, announcement, studentAnnouncements, mapper);
        }

        protected AnnouncementGradeViewData(AnnouncementComplex announcement, bool? wasAnnouncementTypeGraded = null)
            : base(announcement, wasAnnouncementTypeGraded)
        {
        }

        public static AnnouncementGradeViewData Create(AnnouncementComplex announcement, IList<StudentAnnouncement> studentAnnouncements, IGradingStyleMapper mapper, bool? wasAnnouncementTypeGraded = null)
        {
            var res = new AnnouncementGradeViewData(announcement, studentAnnouncements, mapper, wasAnnouncementTypeGraded);
            return res;
        }

        private static void PrepareGradingInfo(AnnouncementViewData res, AnnouncementComplex announcement, IList<StudentAnnouncement> studentAnnouncements, IGradingStyleMapper mapper)
        {
            res.Avg = announcement.Avg;
            res.AvgNumeric = announcement.Avg;
            res.GradingStyle = (int)announcement.GradingStyle;
            if (studentAnnouncements != null && studentAnnouncements.Count > 0)
            {
                if (studentAnnouncements.Count == 1)
                {
                    var studentAnnouncement = studentAnnouncements.First();
                    res.Grade = studentAnnouncement.NumericScore;
                    res.Comment = studentAnnouncement.Comment;
                }
                int graded = 0;
                decimal? summ = null;
                int cnt = 0;
                foreach (var gradeItem in studentAnnouncements)
                {
                    if (gradeItem.NumericScore.HasValue)
                    {
                        graded++;
                        summ = summ.HasValue ? summ + gradeItem.NumericScore.Value : gradeItem.NumericScore.Value;
                        cnt++;
                    }
                }
                var count = studentAnnouncements.Count;
                res.GradeSummary = graded + "/" + count;
                res.AttachmentSummary = res.StudentsCountWithAttachments + "/" + count;
                res.Avg = summ.HasValue ? (int)Math.Round(1.0 * (double)summ.Value / cnt) : (int?)null;
                res.AvgNumeric = summ.HasValue ? Math.Round(1.0 * (double)summ.Value / cnt) : (double?)null;
            }
        }
    }
}