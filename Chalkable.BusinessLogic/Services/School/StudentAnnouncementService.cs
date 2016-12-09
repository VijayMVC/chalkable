using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IStudentAnnouncementService
    {
        IList<StudentAnnouncementDetails> GetStudentAnnouncements(int announcementId);
        Task<IList<StudentAnnouncementDetails>> GetStudentAnnouncementsByAnnIds(IList<int> announcementIds, int studentId);
        Task<IList<StudentAnnouncementDetails>> GetStudentAnnouncementsForGradingPeriod(int schoolYearId, int studentId, int gradingPeriodId);
        StudentAnnouncement SetGrade(int announcementId, int studentId, string value, string extraCredits, string comment
            , bool dropped, bool late, bool exempt, bool incomplete, GradingStyleEnum? gradingStyle = null);
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


        public async Task<IList<StudentAnnouncementDetails>> GetStudentAnnouncementsForGradingPeriod(int schoolYearId, int studentId, int gradingPeriodId)
        {
            var gradingDetailsDashboardTask =  ConnectorLocator.GradingConnector.GetStudentGradingDetails(schoolYearId, studentId, gradingPeriodId);
            var student = ServiceLocator.StudentService.GetById(studentId, schoolYearId);
            var scores = (await gradingDetailsDashboardTask).Scores;

            var mapper = MapperFactory.GetMapper<StudentAnnouncement, Score>();
            var res = new List<StudentAnnouncementDetails>();
            foreach (var score in scores)
            {
                var studentAnn = new StudentAnnouncementDetails
                {
                    Student = student,
                    StudentId = studentId
                };
                mapper.Map(studentAnn, score);
                res.Add(studentAnn);
            }
            return res;
        }

        public StudentAnnouncement SetGrade(int announcementId, int studentId, string value, string extraCredits, string comment, bool dropped,
                                            bool late, bool exempt, bool incomplete, GradingStyleEnum? gradingStyle = null)
        {
            var ann = ServiceLocator.ClassAnnouncementService.GetClassAnnouncemenById(announcementId);
            if(!ann.IsSubmitted)
                throw new ChalkableException("Announcement is not submitted yet");

            if (!string.IsNullOrEmpty(value) && value.Trim() != "")
                exempt = false;

            else value = null;

            //need this info, before we post data to iNow, for notification
            Trace.Assert(ann.SisActivityId.HasValue);
            var studentScoreBefore = Task.Run(()=>ConnectorLocator.ActivityScoreConnector.GetScore(ann.SisActivityId.Value, studentId)).Result;

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

            //
            var commentWasChanged = (!string.IsNullOrWhiteSpace(comment) && studentScoreBefore.Comment    != comment);
            var scoreWasChanged =   (!string.IsNullOrWhiteSpace(value)   && studentScoreBefore.ScoreValue != value);

            if (ann.VisibleForStudent && (commentWasChanged || scoreWasChanged))
                ServiceLocator.NotificationService.AddAnnouncementSetGradeNotificationToStudent(announcementId, stAnn.StudentId);

            return stAnn;
        }


        public async Task<IList<StudentAnnouncementDetails>> GetStudentAnnouncementsByAnnIds(IList<int> announcementIds, int studentId)
        {
            var tasks = announcementIds.Select(annId => GetStudentAnnouncement(annId, studentId)).ToArray();
            var res = await Task.WhenAll(tasks);
            return res.ToList();
        }
        
        private async Task<StudentAnnouncementDetails> GetStudentAnnouncement(int announcementId, int studentId)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            var ann = ServiceLocator.ClassAnnouncementService.GetClassAnnouncemenById(announcementId);
            if (!ann.SisActivityId.HasValue)
                throw new ChalkableException("Current announcement is not in Inow ");

            var scoreTask = ConnectorLocator.ActivityScoreConnector.GetScore(ann.SisActivityId.Value, studentId);
            var student = ServiceLocator.StudentService.GetById(studentId, Context.SchoolYearId.Value);
            var alternateScores = ServiceLocator.AlternateScoreService.GetAlternateScores();
            return CreateStudentAnnouncement(ann, student, await scoreTask, alternateScores);
        }


        //TODO: make this method async 
        public IList<StudentAnnouncementDetails> GetStudentAnnouncements(int announcementId)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();

            if (CoreRoles.STUDENT_ROLE == Context.Role)
                return new List<StudentAnnouncementDetails> { Task.Run(() => GetStudentAnnouncement(announcementId, Context.PersonId.Value)).Result};

            Trace.Assert(Context.SchoolYearId.HasValue);
            var ann = ServiceLocator.ClassAnnouncementService.GetClassAnnouncemenById(announcementId);

            if (!ann.SisActivityId.HasValue)
                throw new ChalkableException("Current announcement is not in Inow ");

            var scoresTask = Task.Run(()=>ConnectorLocator.ActivityScoreConnector.GetScores(ann.SisActivityId.Value));
            var classRoomOption = ServiceLocator.ClassroomOptionService.GetClassOption(ann.ClassRef);
            var enrolled = classRoomOption != null && !classRoomOption.IncludeWithdrawnStudents ? true : default(bool?);
            var mp = ServiceLocator.MarkingPeriodService.GetLastClassMarkingPeriod(ann.ClassRef, ann.Expires);
            if (mp == null)
                throw new ChalkableException("No marking period is scheduled at announcements expiery date.");
            
            var students = ServiceLocator.StudentService.GetClassStudents(ann.ClassRef, mp.Id, enrolled);
            var alternateScores = ServiceLocator.AlternateScoreService.GetAlternateScores();
            return CreateStudentAnnouncements(ann, students, scoresTask.Result, alternateScores);
        }

        private StudentAnnouncementDetails CreateStudentAnnouncement(ClassAnnouncement ann, Student student, Score score, IList<AlternateScore> alternateScores)
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
            return stAnn;
        }

        private IList<StudentAnnouncementDetails> CreateStudentAnnouncements(ClassAnnouncement ann, IList<Student> students, IList<Score> scores, IList<AlternateScore> alternateScores)
        {
            var res = new List<StudentAnnouncementDetails>();
            foreach (var score in scores)
            {
                var student = students.FirstOrDefault(x => x.Id == score.StudentId);
                if (student != null)
                {
                    res.Add(CreateStudentAnnouncement(ann, student, score, alternateScores));
                }
            }
            return res;
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
