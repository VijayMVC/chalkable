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
        StudentAnnouncement SetGrade(int announcementId, int studentId, string value, string extraCredits, string comment
            , bool dropped, bool late, bool exempt, bool incomplete, bool commentWasChanged, GradingStyleEnum? gradingStyle = null);
        AutoGrade SetAutoGrade(int announcementApplicationId, int? recepientId, string value);
        IList<AutoGrade> GetAutoGradesByAnnouncementId(int announcementId);
        IList<AutoGrade> GetAutoGrades(int announcementApplicationId);
    }

    public class StudentAnnouncementService : SisConnectedService, IStudentAnnouncementService
    {
        public StudentAnnouncementService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        //TODO : needs testing 
        public StudentAnnouncement SetGrade(int announcementId, int studentId, string value, string extraCredits, string comment, bool dropped,
                                            bool late, bool exempt, bool incomplete, bool commentWasChanged, GradingStyleEnum? gradingStyle = null)
        {
            var ann = ServiceLocator.ClassAnnouncementService.GetClassAnnouncemenById(announcementId);
            if(!ann.IsSubmitted)
                throw new ChalkableException("Announcement is not submitted yet");
            if (!string.IsNullOrEmpty(value) && value.Trim() != "")
                exempt = false;
            else value = null;
            
            var stAnn = new StudentAnnouncement
            {
                ExtraCredit = extraCredits,
                Comment = comment,
                ScoreDropped = dropped,
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
            
            if (ann.VisibleForStudent && !string.IsNullOrWhiteSpace(value) && !(commentWasChanged && string.IsNullOrWhiteSpace(comment)))
                ServiceLocator.NotificationService.AddAnnouncementSetGradeNotificationToStudent(announcementId, stAnn.StudentId);
            return stAnn;
        }

        public IList<StudentAnnouncementDetails> GetStudentAnnouncements(int announcementId)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            Trace.Assert(Context.SchoolYearId.HasValue);
            var ann = ServiceLocator.ClassAnnouncementService.GetClassAnnouncemenById(announcementId);
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

        public AutoGrade SetAutoGrade(int announcementApplicationId, int? studentId, string value)
        {
            var recepientId = studentId ?? Context.PersonId ?? 0;
            var annApp = ServiceLocator.ApplicationSchoolService.GetAnnouncementApplication(announcementApplicationId);
            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationByUrl(Context.OAuthApplication);
            
            if(annApp.ApplicationRef != app.Id)
                throw new ChalkableSecurityException("There is no AnnouncementApplication with such Id and ApplicationId");

            if(!annApp.Active)
                throw new ChalkableSecurityException("This application is not attached to an item");

            var announcement = ServiceLocator.ClassAnnouncementService.GetClassAnnouncemenById(annApp.AnnouncementRef); // security here

            if (recepientId != Context.PersonId && !announcement.IsOwner)
                throw new ChalkableSecurityException("Only owner can post auto grade for student");

            using (var uow = Update())
            {
                var da = new AutoGradeDataAccess(uow);
                Action<AutoGrade> modifyAction = da.Update;
                var autoGrade = da.GetAutoGrade(announcementApplicationId, recepientId);
                if (autoGrade == null)
                {
                    autoGrade = new AutoGrade
                    {
                        AnnouncementApplicationRef = announcementApplicationId,
                        StudentRef = recepientId,
                    };
                    modifyAction = da.Insert;
                }
                autoGrade.Date = Context.NowSchoolTime;
                autoGrade.Grade = value;
                autoGrade.Posted = false;
                autoGrade.AnnouncementApplication = annApp;
                modifyAction(autoGrade);
                uow.Commit();
                return autoGrade;
            }
        }
        
        public IList<AutoGrade> GetAutoGradesByAnnouncementId(int announcementId)
        {
            return DoRead(uow => new AutoGradeDataAccess(uow).GetAutoGradesByAnnouncementId(announcementId));
        }

        public IList<AutoGrade> GetAutoGrades(int announcementApplicationId)
        {
            return DoRead(uow => new AutoGradeDataAccess(uow).GetAutoGrades(announcementApplicationId));
        }
    }
}
