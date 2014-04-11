using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IStudentAnnouncementService
    {
        IList<StudentAnnouncementDetails> GetStudentAnnouncements(int announcementId);
        void ResolveAutoGrading(int announcementId, bool apply);
        //IList<StudentAnnouncement> GetStudentAnnouncements(int schoolPersonId, int classId);
        StudentAnnouncement SetGrade(int announcementId, int studentId, string value, string extraCredits, string comment
            , bool dropped, bool late, bool exempt, bool incomplete, GradingStyleEnum? gradingStyle = null);
        //StudentAnnouncementInfo SetAutoGrade(int studentAnnouncementId, int value, Guid applicationId);
        //IList<StudentGradingComplex> GetStudentGradedAnnouncements(int schoolPersonId, int markingPeriodId);

        //int? GetAssignmentAverage(int announcementId);
        //double GetAvgByAnnouncements(IList<StudentAnnouncement> studentAnnouncements, bool dropLowest);

        //IList<StudentAnnouncementGrade> GetLastGrades(int studentId, int? classId = null, int count = int.MaxValue);

    }

    public class StudentAnnouncementService : SisConnectedService, IStudentAnnouncementService
    {
        public StudentAnnouncementService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        //TODO : needs testing 
        public StudentAnnouncement SetGrade(int announcementId, int studentId, string value, string extraCredits, string comment, bool dropped,
                                            bool late, bool exempt, bool incomplete, GradingStyleEnum? gradingStyle = null)
        {
            var ann = ServiceLocator.AnnouncementService.GetAnnouncementById(announcementId);
            var stAnn = new StudentAnnouncement
            {
                ExtraCredit = extraCredits,
                Comment = comment.Trim(),
                Dropped = dropped,
                Incomplete = incomplete,
                Late = late,
                Exempt = exempt,
                ActivityId = ann.SisActivityId.Value,
                AnnouncementId = announcementId,
                StudentId = studentId,
                ScoreValue = value,
            };
            var score = new Score();
            MapperFactory.GetMapper<Score, StudentAnnouncement>().Map(score, stAnn);
            score = ConnectorLocator.ActivityScoreConnector.UpdateScore(score.ActivityId, score.StudentId, score);
            MapperFactory.GetMapper<StudentAnnouncement, Score>().Map(stAnn, score);
            ServiceLocator.NotificationService.AddAnnouncementSetGradeNotificationToPerson(announcementId, stAnn.StudentId);
            return stAnn;
        }

        public IList<StudentAnnouncementDetails> GetStudentAnnouncements(int announcementId)
        {
            if(!Context.UserLocalId.HasValue)
                throw new UnassignedUserException();
            var ann = ServiceLocator.AnnouncementService.GetAnnouncementById(announcementId);
            if (!ann.ClassRef.HasValue)
                throw new ChalkableException("Announcement is not assigned to any class");
            if (ann.SisActivityId.HasValue)
            {
                IList<Score> scores = new List<Score>();
                IList<Person> persons = new List<Person>();
                if (CoreRoles.STUDENT_ROLE == Context.Role)
                {
                    scores.Add(ConnectorLocator.ActivityScoreConnector.GetScore(ann.SisActivityId.Value, Context.UserLocalId.Value));
                    persons.Add(ServiceLocator.PersonService.GetPerson(Context.UserLocalId.Value));
                }
                else
                {
                    scores = ConnectorLocator.ActivityScoreConnector.GetSores(ann.SisActivityId.Value);
                    persons = ServiceLocator.PersonService.GetPaginatedPersons(new PersonQuery { ClassId = ann.ClassRef });
                }
                var res = new List<StudentAnnouncementDetails>();
                foreach (var score in scores)
                {
                    var stAnn = new StudentAnnouncementDetails
                        {
                            ClassId = ann.ClassRef.Value,
                            Student = persons.First(x=>x.Id == score.StudentId),
                            AnnouncementId = ann.Id
                        };
                    MapperFactory.GetMapper<StudentAnnouncementDetails, Score>().Map(stAnn, score);    
                    res.Add(stAnn);
                }
                return res;

            }
            throw new ChalkableException("Current announcement is not in Inow ");

        }

        //TODO: check application existing  
        public StudentAnnouncement SetAutoGrade(int studentAnnouncementId, int value, Guid applicationId)
        {
            //using (var uow = Update())
            //{
            //    var da = new StudentAnnouncementDataAccess(uow);
            //    var sa = da.GetById(studentAnnouncementId);
            //    if (sa.State == StudentAnnouncementStateEnum.None)
            //    {
            //        sa.GradeValue = value;
            //        sa.ApplicationRef = applicationId;
            //        sa.State = StudentAnnouncementStateEnum.Auto;
            //        da.Update(sa);
            //    }
            //    uow.Commit();
            //    return sa;
            //}
            throw new NotImplementedException();
        }


        public void ResolveAutoGrading(int announcementId, bool apply)
        {
            //using (var uow = Update())
            //{
            //    if (!BaseSecurity.IsAdminOrTeacher(ServiceLocator.Context))
            //        throw new ChalkableSecurityException();
            //    var state = StudentAnnouncementStateEnum.Auto;
            //    var da = new StudentAnnouncementDataAccess(uow);
            //    var sas = da.GetList(new StudentAnnouncementShortQuery {AnnouncementId = announcementId, State = state});
            //    foreach (var studentAnnouncement in sas)
            //    {
            //        studentAnnouncement.State = StudentAnnouncementStateEnum.Manual;
            //        if (!apply)
            //            studentAnnouncement.GradeValue = null;
            //    }
            //    da.Update(sas);
            //    uow.Commit();
            //}
            throw new NotImplementedException();
        }


        //public IList<StudentAnnouncementGrade> GetLastGrades(int studentId, int? classId = null, int count = int.MaxValue)
        //{
        //    using (var uow = Read())
        //    {
        //        return new StudentAnnouncementDataAccess(uow).GetStudentAnnouncementGrades(new StudentAnnouncementQuery
        //            {
        //                StudentId = studentId,
        //                ClassId = classId,
        //                Count = count
        //            });
        //    }
        //}
        
        
    }
}
