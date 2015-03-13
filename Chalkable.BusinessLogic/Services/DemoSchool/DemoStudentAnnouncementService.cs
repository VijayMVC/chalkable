using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{

    public class DemoStudentAnnouncementService : DemoSchoolServiceBase, IStudentAnnouncementService
    {
        public DemoStudentAnnouncementService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public StudentAnnouncement SetGrade(int announcementId, int studentId, string value, string extraCredits, 
            string comment, bool dropped, bool late, bool exempt, bool incomplete, GradingStyleEnum? gradingStyle = null)
        {
            var ann = ServiceLocator.AnnouncementService.GetAnnouncementById(announcementId);

            Trace.Assert(ann.SisActivityId.HasValue);

            if (!string.IsNullOrEmpty(value) && value.Trim() != "")
                exempt = false;
            else 
                value = null;

            decimal numericScore;
            var isDecimal = decimal.TryParse(value, out numericScore);

            if (value != null && !isDecimal)
            {
                var numScore = Storage.AlphaGradeStorage.GetAll()
                    .FirstOrDefault(x => String.Equals(x.Name, value, StringComparison.InvariantCultureIgnoreCase));
                if (numScore != null)
                {
                    var gradingScaleRange = Storage.GradingScaleRangeStorage
                        .GetAll()
                        .FirstOrDefault(x => x.AlphaGradeRef == numScore.Id);

                    if (gradingScaleRange != null)
                    {
                        numericScore = gradingScaleRange.AveragingEquivalent;
                        value = numScore.Name;
                    }
                        
                }
            }


            var gradeComment = comment != null && !string.IsNullOrWhiteSpace(comment) ? comment.Trim() : "";

            var oldScore = Storage.StiActivityScoreStorage.GetScore(announcementId, studentId);
            var studentAnnouncement = new StudentAnnouncement();

            MapperFactory.GetMapper<StudentAnnouncement, Score>().Map(studentAnnouncement, oldScore);
            studentAnnouncement.AnnouncementId = oldScore.ActivityId;

            if (numericScore >= 0 && value != null)
            {
                studentAnnouncement.NumericScore = numericScore;
                studentAnnouncement.ScoreValue = value.ToString(CultureInfo.InvariantCulture);
            }
            else if (value == null)
            {
                studentAnnouncement.NumericScore = null;
                studentAnnouncement.ScoreValue = "";
            }

            if (studentAnnouncement.NumericScore.HasValue || studentAnnouncement.Late ||
                studentAnnouncement.Incomplete)
            {
                studentAnnouncement.ScoreDropped = dropped;
                studentAnnouncement.Comment = gradeComment;
            }

            studentAnnouncement.Exempt = exempt;
            studentAnnouncement.Late = late;
            studentAnnouncement.Incomplete = incomplete;
            studentAnnouncement.ExtraCredit = extraCredits;
            
            var score = new Score();
            MapperFactory.GetMapper<Score, StudentAnnouncement>().Map(score, studentAnnouncement);
            score = Storage.StiActivityScoreStorage.UpdateScore(score.ActivityId, score.StudentId, score);
            score.ActivityDate = ann.Expires;
            score.ActivityName = ann.Title;
            MapperFactory.GetMapper<StudentAnnouncement, Score>().Map(studentAnnouncement, score);

            if (studentAnnouncement.AlternateScoreId.HasValue)
                studentAnnouncement.AlternateScore = ServiceLocator.AlternateScoreService.GetAlternateScore(studentAnnouncement.AlternateScoreId.Value);
            if (ann.VisibleForStudent && !string.IsNullOrWhiteSpace(value))
                ServiceLocator.NotificationService.AddAnnouncementSetGradeNotificationToStudent(announcementId, studentAnnouncement.StudentId);
            return studentAnnouncement;
        }

        public IList<StudentAnnouncementDetails> GetStudentAnnouncements(int announcementId)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            Trace.Assert(Context.SchoolYearId.HasValue);
            var ann = ServiceLocator.AnnouncementService.GetAnnouncementById(announcementId);
            var mp = ServiceLocator.MarkingPeriodService.GetMarkingPeriodByDate(ann.Expires, true);
            Trace.Assert(mp != null);
            if (ann.SisActivityId.HasValue)
            {
                IList<Score> scores = new List<Score>();
                IList<StudentDetails> persons = new List<StudentDetails>();
                if (CoreRoles.STUDENT_ROLE == Context.Role)
                {
                    scores.Add(Storage.StiActivityScoreStorage.GetScore(ann.SisActivityId.Value, Context.PersonId.Value));
                    persons.Add(ServiceLocator.StudentService.GetById(Context.PersonId.Value, Context.SchoolYearId.Value));
                }
                else
                {
                    scores = Storage.StiActivityScoreStorage.GetSores(ann.SisActivityId.Value);
                    persons = ServiceLocator.StudentService.GetClassStudents(ann.ClassRef, mp.Id);
                }
                var res = new List<StudentAnnouncementDetails>();
                foreach (var score in scores)
                {
                    var stAnn = new StudentAnnouncementDetails
                    {
                        ClassId = ann.ClassRef,
                        Student = persons.First(x => x.Id == score.StudentId),
                        AnnouncementId = ann.Id
                    };
                    MapperFactory.GetMapper<StudentAnnouncementDetails, Score>().Map(stAnn, score);
                    res.Add(stAnn);
                }
                return res;

            }
            throw new ChalkableException("Current announcement is not in Inow ");

        }

        public AutoGrade SetAutoGrade(int announcementApplicationId, int? recepientId, string value)
        {
            var annApp = ServiceLocator.ApplicationSchoolService.GetAnnouncementApplication(announcementApplicationId);
            var autoGrade = Storage.AutoGradeStorage.GetAll()
                                   .FirstOrDefault(x => x.AnnouncementApplicationRef == announcementApplicationId
                                                        && x.StudentRef == recepientId);
            if (autoGrade == null)
            {
                autoGrade = new AutoGrade
                    {
                        AnnouncementApplicationRef = announcementApplicationId,
                        StudentRef = recepientId ?? Context.PersonId ?? 0,
                        AnnouncementApplication = annApp,
                        Date = Context.NowSchoolYearTime,
                        Grade = value,
                    };
                Storage.AutoGradeStorage.Add(autoGrade);
            }
            else
            {
                autoGrade.Grade = value;
                autoGrade.Date = Context.NowSchoolYearTime;
                Storage.AutoGradeStorage.SetAutoGrade(autoGrade);
            }
            return autoGrade;
        }

        public IList<AutoGrade> GetAutoGradesByAnnouncementId(int announcementId)
        {
            var annApps = ServiceLocator.ApplicationSchoolService.GetAnnouncementApplicationsByAnnId(announcementId);
            return Storage.AutoGradeStorage.GetAll()
                       .Where(x => annApps.Any(y => y.Id == x.AnnouncementApplicationRef))
                       .ToList();
        }

        public IList<AutoGrade> GetAutoGrades(int announcementApplicationId)
        {
            var autogrades =
                Storage.AutoGradeStorage.GetAll()
                    .Where(x => x.AnnouncementApplication.Id == announcementApplicationId)
                    .Select(x => x)
                    .ToList();
            return autogrades;
        }
    }
}
