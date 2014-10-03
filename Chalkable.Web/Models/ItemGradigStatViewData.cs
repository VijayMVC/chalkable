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
        public IList<ItemGradingGraphViewData> GraphPoints { get; set; }
        public IList<StudentAnnouncementDetails> StudentAnnouncements { get; set; }

        private const int INTERVALS_COUNT = 4;
        private const int MAX_GRADE = 100;

        public static ItemGradigStatViewData Create(IList<StudentAnnouncementDetails> studentAnnouncements, int announcementId)
        {
            var res = new ItemGradigStatViewData();
            if (studentAnnouncements.Count > 0)
            {
                var grades = studentAnnouncements.Where(x => x.NumericScore.HasValue).Select(x => x.NumericScore.Value).ToList();
                var graphPoints = new List<ItemGradingGraphViewData>();
                if (grades.Count > 0)
                {
                    double minGrade = (double)grades.Min();
                    double intervalLength = (MAX_GRADE - minGrade) / INTERVALS_COUNT;
                    if (intervalLength < 1)
                        intervalLength = MAX_GRADE - minGrade;
                    if ((int)minGrade == MAX_GRADE)
                    {
                        graphPoints.Add(ItemGradingGraphViewData.Create((int)minGrade, grades.Count, (int)minGrade, (int)minGrade));
                    }
                    double position = minGrade;
                    while (position < MAX_GRADE)
                    {
                        var startInterval = position;
                        position += intervalLength;
                        var endInterval = position;
                        var grade = (startInterval + position) / 2;
                        var studentCount = grades.Count(x => (double)x >= startInterval && (double)x < position);
                        graphPoints.Add(ItemGradingGraphViewData.Create((int)grade, studentCount, (int)startInterval, (int)endInterval));
                    }
                }
                res.AnnouncementId = announcementId;
                res.GraphPoints = graphPoints;
            }
            res.StudentAnnouncements = studentAnnouncements;
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

        public static ItemGradingGraphViewData Create(int grade, int studentCount, int startInterval, int endInterval)
        {
            return new ItemGradingGraphViewData
            {
                Grade = grade,
                StudentCount = studentCount,
                StartInterval = startInterval,
                EndInterval = endInterval,
                MappedStartInterval = startInterval,
                MappedEndInterval = endInterval,
            };
        }
    }
}