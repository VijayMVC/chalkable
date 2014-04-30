using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStudentAnnouncementStorage:BaseDemoStorage<int, StudentAnnouncement>
    {
        public DemoStudentAnnouncementStorage(DemoStorage storage) : base(storage)
        {
        }

        public void Add(StudentAnnouncement an)
        {
            data.Add(GetNextFreeId(), an);
        }

        public void Add(IList<StudentAnnouncement> studentAnnouncements)
        {
            foreach (var studentAnnouncement in studentAnnouncements)
            {
                Add(studentAnnouncement);
            }
        }

        public void Update(StudentAnnouncement sa)
        {
            var studentAnnouncement = data.First(x => x.Value == sa);
            data[studentAnnouncement.Key] = sa;
        }

        public void Update(IList<StudentAnnouncement> announcements)
        {
            foreach (var studentAnnouncement in announcements)
            {
                Update(studentAnnouncement);
            }
        }

        public void Update(int announcementId, bool drop)
        {
            var sa = data.Where(x => x.Value.AnnouncementId == announcementId).Select(x => x.Key).First();
            data[sa].Dropped = drop;
        }

        public IList<StudentAnnouncementDetails> GetAll(int announcementId)
        {
            return data.Where(x => x.Value.AnnouncementId == announcementId).Select(x =>
            {
                var ann = Storage.AnnouncementStorage.GetById(announcementId);
                var classId = ann.ClassRef.HasValue ? ann.ClassRef.Value : 0;
                return new StudentAnnouncementDetails
                {
                    AnnouncementId = x.Value.AnnouncementId,
                    AbsenceCategory = x.Value.AbsenceCategory,
                    Absent = x.Value.Absent,
                    ActivityId = x.Value.ActivityId,
                    AlphaGradeId = x.Value.AlphaGradeId,
                    AlternateScoreId = x.Value.AlternateScoreId,
                    Comment = x.Value.Comment,
                    Dropped = x.Value.Dropped,
                    Exempt = x.Value.Exempt,
                    ExtraCredit = x.Value.ExtraCredit,
                    Incomplete = x.Value.Incomplete,
                    ClassId = classId,
                    Late = x.Value.Late,
                    NumericScore = x.Value.NumericScore,
                    OverMaxScore = x.Value.OverMaxScore,
                    ScoreValue = x.Value.ScoreValue,
                    Student = Storage.PersonStorage.GetById(x.Value.StudentId),
                    StudentId = x.Value.StudentId,
                    Withdrawn = x.Value.Withdrawn
                };

            }).ToList();

        }

        public override void Setup()
        {
            throw new System.NotImplementedException();
        }
    }
}
