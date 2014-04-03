using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStudentAnnouncementStorage:BaseDemoStorage<int, StudentAnnouncement>
    {
        private int index = 0;

        public DemoStudentAnnouncementStorage(DemoStorage storage) : base(storage)
        {
        }

        public void Add(StudentAnnouncement an)
        {
            data.Add(index++, an);
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

        public IList<StudentAnnouncementDetails> GetStudentAnnouncementsDetails(int announcementId, int i)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }
    }
}
