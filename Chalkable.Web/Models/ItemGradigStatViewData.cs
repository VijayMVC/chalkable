using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Mapping;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class ItemGradigStatViewData
    {
        public int AnnouncementId { get; set; }
        public int GradingStyle { get; set; }
        public IList<ItemGradingGraphViewData> GraphPoints { get; set; }

        private const int INTERVALS_COUNT = 4;
        private const int MAX_GRADE = 100;

        public static ItemGradigStatViewData Create(IList<StudentAnnouncementDetails> studentAnnouncements, Announcement announcement, IGradingStyleMapper mapper)
        {
            var res = new ItemGradigStatViewData();
            if (studentAnnouncements.Count > 0)
            {
                var grades = studentAnnouncements.Where(x => x.GradeValue.HasValue).Select(x => x.GradeValue.Value).ToList();
                var graphPoints = new List<ItemGradingGraphViewData>();
                if (grades.Count > 0)
                {
                    double minGrade = grades.Min();
                    double intervalLength = (MAX_GRADE - minGrade) / INTERVALS_COUNT;
                    if (intervalLength < 1)
                        intervalLength = MAX_GRADE - minGrade;
                    if ((int)minGrade == MAX_GRADE)
                    {
                        graphPoints.Add(ItemGradingGraphViewData.Create(announcement, (int)minGrade, grades.Count, (int)minGrade, (int)minGrade, mapper));
                    }
                    double position = minGrade;
                    while (position < MAX_GRADE)
                    {
                        var startInterval = position;
                        position += intervalLength;
                        var endInterval = position;
                        var grade = (startInterval + position) / 2;
                        var studentCount = grades.Count(x => x >= startInterval && x < position);
                        graphPoints.Add(ItemGradingGraphViewData.Create(announcement, (int)grade, studentCount, (int)startInterval, (int)endInterval, mapper));
                    }
                }
                res.AnnouncementId = announcement.Id;
                res.GradingStyle = (int)announcement.GradingStyle;
                res.GraphPoints = graphPoints;

            }
            return res;
        }
    }
    public class ItemGradingGraphViewData
    {
        public int Grade { get; set; }
        public int StudentCount { get; set; }
        public int StartInterval { get; set; }
        public int EndInterval { get; set; }
        public int GradingStyle { get; set; }
        public int MappedStartInterval { get; set; }
        public int MappedEndInterval { get; set; }

        public static ItemGradingGraphViewData Create(Announcement announcement, int grade, int studentCount, int startInterval,
                                               int endInterval, IGradingStyleMapper mapper)
        {
            return new ItemGradingGraphViewData
            {
                Grade = grade,
                StudentCount = studentCount,
                StartInterval = startInterval,
                EndInterval = endInterval,
                GradingStyle = (int)announcement.GradingStyle,
                MappedStartInterval = mapper.Map(announcement.GradingStyle, startInterval).Value,
                MappedEndInterval = mapper.Map(announcement.GradingStyle, endInterval).Value,
            };
        }
    }
}