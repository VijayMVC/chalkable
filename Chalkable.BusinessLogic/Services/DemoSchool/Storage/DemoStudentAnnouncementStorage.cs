using System.Collections;
using System.Collections.Generic;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStudentAnnouncementStorage
    {
        public void Add(StudentSchoolYear studentSchoolYear)
        {
            throw new System.NotImplementedException();
        }

        public void Add(IList<StudentSchoolYear> studentSchoolYear)
        {
            throw new System.NotImplementedException();
        }

        public IList<StudentSchoolYear> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public StudentAnnouncement GetById(int studentAnnouncementId)
        {
            throw new System.NotImplementedException();
        }

        public void Update(StudentAnnouncement sa)
        {
            throw new System.NotImplementedException();
        }

        public IList<StudentAnnouncementDetails> GetStudentAnnouncementsDetails(int announcementId, int i)
        {
            throw new System.NotImplementedException();
        }

        public void Update(List<StudentAnnouncement> notGraded)
        {
            throw new System.NotImplementedException();
        }

        public IList<StudentAnnouncement> GetList(StudentAnnouncementShortQuery studentAnnouncementShortQuery)
        {
            throw new System.NotImplementedException();
        }

        public void Update(IList<StudentAnnouncement> notGraded)
        {
            throw new System.NotImplementedException();
        }

        public IList<StudentAnnouncementGrade> GetStudentAnnouncementGrades(StudentAnnouncementQuery studentAnnouncementQuery)
        {
            throw new System.NotImplementedException();
        }
    }
}
