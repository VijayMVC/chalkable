using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
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
        StudentAnnouncement SetGrade(int studentAnnouncementId, string value, string extraCredits, string comment
            , bool dropped, bool late, bool absent, bool exempt, bool incomplete, GradingStyleEnum? gradingStyle = null);
        StudentAnnouncement SetAutoGrade(int studentAnnouncementId, int value, Guid applicationId);
        //IList<StudentGradingComplex> GetStudentGradedAnnouncements(int schoolPersonId, int markingPeriodId);

        //int? GetAssignmentAverage(int announcementId);
        //double GetAvgByAnnouncements(IList<StudentAnnouncement> studentAnnouncements, bool dropLowest);

        IList<StudentAnnouncementGrade> GetLastGrades(int studentId, int? classId = null, int count = int.MaxValue);
    }

    public class StudentAnnouncementService : SisConnectedService, IStudentAnnouncementService
    {
        public StudentAnnouncementService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        //TODO : needs testing 
        public StudentAnnouncement SetGrade(int studentAnnouncementId, string value, string extraCredits, string comment, bool dropped,
                                            bool late, bool absent, bool exempt, bool incomplete, GradingStyleEnum? gradingStyle = null)
        {
            using (var uow = Update())
            {
                var saDa = new StudentAnnouncementDataAccess(uow);
                var sa = saDa.GetById(studentAnnouncementId);
                var ann = ServiceLocator.AnnouncementService.GetAnnouncementById(sa.AnnouncementRef);
                if(!AnnouncementSecurity.CanModifyAnnouncement(ann, Context))
                    throw new ChalkableSecurityException();

                sa.ExtraCredit = extraCredits;
                sa.Comment = comment.Trim();
                sa.State = StudentAnnouncementStateEnum.Manual;
                sa.Dropped = dropped;
                sa.Incomplete = incomplete;
                sa.Late = late;
                sa.Exempt = exempt;
                sa.Absent = absent;
                
                sa.StiScoreValue = value;
                if (!string.IsNullOrEmpty(value))
                {
                    int number;
                    if (int.TryParse(value, out number))
                    {
                        //TODO:remove this later
                        var mapper = ServiceLocator.GradingStyleService.GetMapper();
                        sa.GradeValue = gradingStyle.HasValue
                                            ? mapper.MapBack(gradingStyle.Value, number)
                                            : mapper.MapBack(ann.GradingStyle, number);
                    }

                }
                else sa.GradeValue = null;
                saDa.Update(sa);
                var score = new Score {ActivityId = ann.SisActivityId.Value};
                MapperFactory.GetMapper<Score, StudentAnnouncement>().Map(score, sa);
                score = ConnectorLocator.ActivityScoreConnector.UpdateScore(score.ActivityId, score.StudentId, score);
                MapperFactory.GetMapper<StudentAnnouncement, Score>().Map(sa, score);
                uow.Commit();
                var recipientId = sa.PersonRef;
                ServiceLocator.NotificationService.AddAnnouncementSetGradeNotificationToPerson(sa.AnnouncementRef, recipientId);
                return sa;
            }
        }

        public IList<StudentAnnouncementDetails> GetStudentAnnouncements(int announcementId)
        {
            var ann = ServiceLocator.AnnouncementService.GetAnnouncementById(announcementId);
            using (var uow = Update())
            {
                var res = new StudentAnnouncementDataAccess(uow)
                   .GetStudentAnnouncementsDetails(announcementId, Context.UserLocalId ?? 0);
                var notGraded = new List<StudentAnnouncement>();
                if (ann.SisActivityId.HasValue)
                {
                    IList<Score> scores = new List<Score>();
                    if(CoreRoles.STUDENT_ROLE == Context.Role)
                        scores.Add(ConnectorLocator.ActivityScoreConnector.GetScore(ann.SisActivityId.Value, Context.UserLocalId.Value));
                    else
                        scores = ConnectorLocator.ActivityScoreConnector.GetSores(ann.SisActivityId.Value);
                    foreach (var score in scores)
                    {
                        var stAnn = res.FirstOrDefault(x => x.PersonRef == score.StudentId);
                        if (stAnn != null)
                        {
                            stAnn.State = StudentAnnouncementStateEnum.Manual;
                            if(score.NumericScore.HasValue && !stAnn.GradeValue.HasValue)
                                notGraded.Add(stAnn);
                            MapperFactory.GetMapper<StudentAnnouncementDetails, Score>().Map(stAnn, score);
                        }
                    }
                    if(notGraded.Count > 0)
                        new StudentAnnouncementDataAccess(uow).Update(notGraded);
                }
                uow.Commit();
                return res;
            }
        }

        //TODO: check application existing  
        public StudentAnnouncement SetAutoGrade(int studentAnnouncementId, int value, Guid applicationId)
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


        public void ResolveAutoGrading(int announcementId, bool apply)
        {
            using (var uow = Update())
            {
                if (!BaseSecurity.IsAdminOrTeacher(ServiceLocator.Context))
                    throw new ChalkableSecurityException();
                var state = StudentAnnouncementStateEnum.Auto;
                var da = new StudentAnnouncementDataAccess(uow);
                var sas = da.GetList(new StudentAnnouncementShortQuery {AnnouncementId = announcementId, State = state});
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


        public IList<StudentAnnouncementGrade> GetLastGrades(int studentId, int? classId = null, int count = int.MaxValue)
        {
            using (var uow = Read())
            {
                return new StudentAnnouncementDataAccess(uow).GetStudentAnnouncementGrades(new StudentAnnouncementQuery
                    {
                        StudentId = studentId,
                        ClassId = classId,
                        Count = count
                    });
            }
        }
    }
}
