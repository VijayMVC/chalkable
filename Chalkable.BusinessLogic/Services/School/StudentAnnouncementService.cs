using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IStudentAnnouncementService
    {
        IList<StudentAnnouncementDetails> GetStudentAnnouncements(Guid announcementId);
        void ResolveAutoGrading(Guid announcementId, bool apply);
        //IList<StudentAnnouncement> GetStudentAnnouncements(int schoolPersonId, int classId);
        StudentAnnouncement SetGrade(Guid studentAnnouncementId, int? value, string extraCredits, string comment, bool dropped, GradingStyleEnum? gradingStyle = null);
        StudentAnnouncement SetAutoGrade(Guid studentAnnouncementId, int value, Guid applicationId);
        //IList<StudentGradingComplex> GetStudentGradedAnnouncements(int schoolPersonId, int markingPeriodId);

        //int? GetAssignmentAverage(int announcementId);
        //double GetAvgByAnnouncements(IList<StudentAnnouncement> studentAnnouncements, bool dropLowest);
    }

    public class StudentAnnouncementService : SchoolServiceBase, IStudentAnnouncementService
    {
        public StudentAnnouncementService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        //TODO : needs testing 
        public StudentAnnouncement SetGrade(Guid studentAnnouncementId, int? value, string extraCredits, string comment, bool dropped,
                                            GradingStyleEnum? gradingStyle = null)
        {
            using (var uow = Update())
            {
                var saDa = new StudentAnnouncementDataAccess(uow);
                var sa = saDa.GetById(studentAnnouncementId);
                var annDa = new AnnouncementDataAccess(uow);
                var ann = ServiceLocator.AnnouncementService.GetAnnouncementDetails(sa.AnnouncementRef);

                if(!AnnouncementSecurity.CanModifyAnnouncement(ann, Context))
                    throw new ChalkableSecurityException();

                sa.ExtraCredit = extraCredits;
                sa.Comment = comment;
                sa.State = StudentAnnouncementStateEnum.Manual;

                var mapper = ServiceLocator.GradingStyleService.GetMapper();
                sa.GradeValue = gradingStyle.HasValue ? mapper.MapBack(gradingStyle.Value, value)
                                                      : mapper.MapBack(ann.GradingStyle, value);
                sa.Dropped = dropped;
                ann.Dropped = dropped && !ann.StudentAnnouncements.Any(x => !x.Dropped && x.Id != studentAnnouncementId);
                
                saDa.Update(sa);
                annDa.Update(ann);
                uow.Commit();
                var recipientId =  ann.StudentAnnouncements.First(x=>x.Id == sa.Id).Person.Id;
                ServiceLocator.NotificationService.AddAnnouncementSetGradeNotificationToPerson(sa.AnnouncementRef, recipientId);
                return sa;
            }
        }

        public IList<StudentAnnouncementDetails> GetStudentAnnouncements(Guid announcementId)
        {
            using (var uow = Read())
            {
               return  new StudentAnnouncementDataAccess(uow)
                   .GetStudentAnnouncementsDetails(announcementId, Context.UserId);
            }
        }

        //TODO: check application existing  
        public StudentAnnouncement SetAutoGrade(Guid studentAnnouncementId, int value, Guid applicationId)
        {
            using (var uow = Update())
            {
                var da = new StudentAnnouncementDataAccess(uow);
                var sa = da.GetById(studentAnnouncementId);
                if (sa.State == StudentAnnouncementStateEnum.None)
                {
                    sa.GradeValue = value;
                    sa.ApplicationRef = applicationId;
                    sa.State = StudentAnnouncementStateEnum.Auto;
                    da.Update(sa);
                }
                uow.Commit();
                return sa;
            }
        }


        public void ResolveAutoGrading(Guid announcementId, bool apply)
        {
            using (var uow = Update())
            {
                if (!BaseSecurity.IsAdminOrTeacher(ServiceLocator.Context))
                    throw new ChalkableSecurityException();
                var state = StudentAnnouncementStateEnum.Auto;
                var da = new StudentAnnouncementDataAccess(uow);
                var sas = da.GetList(new StudentAnnouncementQuery {AnnouncementId = announcementId, State = state});
                foreach (var studentAnnouncement in sas)
                {
                    studentAnnouncement.State = StudentAnnouncementStateEnum.Manual;
                    if (!apply)
                        studentAnnouncement.GradeValue = null;
                }
                da.Update(sas);
                uow.Commit();
            }
        }
    }
}
