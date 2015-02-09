using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
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
        AutoGrade SetAutoGrade(int announcementApplicationId, int studentId, string value);
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
            if (!string.IsNullOrEmpty(value) && value.Trim() != "")
                exempt = false;
            else value = null;
            
            var stAnn = new StudentAnnouncement
            {
                ExtraCredit = extraCredits,
                Comment = comment,
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
            score.ActivityDate = ann.Expires;
            score.ActivityName = ann.Title;
            score = ConnectorLocator.ActivityScoreConnector.UpdateScore(score.ActivityId, score.StudentId, score);
            MapperFactory.GetMapper<StudentAnnouncement, Score>().Map(stAnn, score);

            if (stAnn.AlternateScoreId.HasValue)
                stAnn.AlternateScore = ServiceLocator.AlternateScoreService.GetAlternateScore(stAnn.AlternateScoreId.Value);
            
            if (ann.VisibleForStudent && !string.IsNullOrWhiteSpace(value))
                ServiceLocator.NotificationService.AddAnnouncementSetGradeNotificationToStudent(announcementId, stAnn.StudentId);
            return stAnn;
        }

        public IList<StudentAnnouncementDetails> GetStudentAnnouncements(int announcementId)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            Trace.Assert(Context.SchoolYearId.HasValue);
            var ann = ServiceLocator.AnnouncementService.GetAnnouncementById(announcementId);
            if (ann.SisActivityId.HasValue)
            {
                IList<Score> scores = new List<Score>();
                IList<StudentDetails> persons = new List<StudentDetails>();
                if (CoreRoles.STUDENT_ROLE == Context.Role)
                {
                    scores.Add(ConnectorLocator.ActivityScoreConnector.GetScore(ann.SisActivityId.Value, Context.PersonId.Value));
                    persons.Add(ServiceLocator.StudentService.GetById(Context.PersonId.Value, Context.SchoolYearId.Value));
                }
                else
                {
                    scores = ConnectorLocator.ActivityScoreConnector.GetSores(ann.SisActivityId.Value);
                    var classRoomOption = ServiceLocator.ClassroomOptionService.GetClassOption(ann.ClassRef);
                    bool? enrolled = classRoomOption != null && !classRoomOption.IncludeWithdrawnStudents ? true : default(bool?);
                    var mp = ServiceLocator.MarkingPeriodService.GetLastMarkingPeriod(ann.Expires);
                    if (mp == null)
                    {
                        throw new ChalkableException("No marking period is scheduled at announcements expiery date.");
                    }
                    persons = ServiceLocator.StudentService.GetClassStudents(ann.ClassRef, mp.Id, enrolled);
                }
                var res = new List<StudentAnnouncementDetails>();
                var alternateScores = ServiceLocator.AlternateScoreService.GetAlternateScores();
                foreach (var score in scores)
                {
                    var student = persons.FirstOrDefault(x => x.Id == score.StudentId);
                    if (student != null)
                    {
                        var stAnn = new StudentAnnouncementDetails
                            {
                                ClassId = ann.ClassRef,
                                Student = student,
                                AnnouncementId = ann.Id
                            };
                        MapperFactory.GetMapper<StudentAnnouncementDetails, Score>().Map(stAnn, score);
                        if (stAnn.AlternateScoreId.HasValue)
                            stAnn.AlternateScore = alternateScores.FirstOrDefault(x => x.Id == stAnn.AlternateScoreId.Value);
                        res.Add(stAnn);    
                    }
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


        public AutoGrade SetAutoGrade(int announcementApplicationId, int studentId, string value)
        {
            if(studentId != Context.PersonId)
                throw new ChalkableSecurityException();

            //TODO: chekc if student has installed current application
            var annApp = ServiceLocator.ApplicationSchoolService.GetAnnouncementApplication(announcementApplicationId);
            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationByUrl(Context.OAuthApplication);
            if(annApp.ApplicationRef == app.Id)
                throw new ChalkableSecurityException("There is no announcemenApplication with such Id and ApplicationId");
            if(!annApp.Active)
                throw new ChalkableSecurityException("Application is not attached to an item");

            ServiceLocator.AnnouncementService.GetAnnouncementById(annApp.AnnouncementRef); // security here

            using (var uow = Update())
            {
                var da = new AutoGradeDataAccess(uow);
                var autoGrade =  da.GetAutoGrade(announcementApplicationId, studentId) ?? new AutoGrade
                    {
                        AnnouncementApplicationRef = announcementApplicationId,
                        StudentRef = studentId
                    };
                autoGrade.Date = Context.NowSchoolTime;
                autoGrade.Grade = value;
                autoGrade.Posted = false;
                return autoGrade;
            }
        }
    }
}
