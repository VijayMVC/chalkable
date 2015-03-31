using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage.sti;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoStudentAnnouncementStorage : BaseDemoIntStorage<StudentAnnouncement>
    {
        public DemoStudentAnnouncementStorage()
            : base(null, true)
        {
        }

        public void Update(int announcementId, bool drop)
        {
            var sa = data.Where(x => x.Value.AnnouncementId == announcementId).Select(x => x.Key).First();
            data[sa].ScoreDropped = drop;
        }

        
    }

    public class DemoStudentAnnouncementService : DemoSchoolServiceBase, IStudentAnnouncementService
    {
        private DemoStudentAnnouncementStorage StudentAnnouncementStorage { get; set; }
        private DemoStiActivityScoreStorage ActivityScoreStorage { get; set; }
        public DemoStudentAnnouncementService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            StudentAnnouncementStorage = new DemoStudentAnnouncementStorage();
            ActivityScoreStorage = new DemoStiActivityScoreStorage();
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
                var numScore = ServiceLocator.AlphaGradeService.GetAlphaGrades()
                    .FirstOrDefault(x => String.Equals(x.Name, value, StringComparison.InvariantCultureIgnoreCase));
                if (numScore != null)
                {
                    var gradingScaleRange = StorageLocator.GradingScaleRangeStorage
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

            var oldScore = ActivityScoreStorage.GetScore(announcementId, studentId);
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
            score = ActivityScoreStorage.UpdateScore(score.ActivityId, score.StudentId, score);
            score.ActivityDate = ann.Expires;
            score.ActivityName = ann.Title;
            MapperFactory.GetMapper<StudentAnnouncement, Score>().Map(studentAnnouncement, score);

            if (studentAnnouncement.AlternateScoreId.HasValue)
                studentAnnouncement.AlternateScore = StorageLocator.AlternateScoreStorage.GetAlternateScore(studentAnnouncement.AlternateScoreId.Value);
            if (ann.VisibleForStudent && !string.IsNullOrWhiteSpace(value))
                ServiceLocator.NotificationService.AddAnnouncementSetGradeNotificationToStudent(announcementId, studentAnnouncement.StudentId);
            return studentAnnouncement;
        }

        public IList<StudentAnnouncementDetails> GetAll(int announcementId)
        {
            return StudentAnnouncementStorage.GetAll().Where(x => x.AnnouncementId == announcementId).Select(x =>
            {
                var ann = ServiceLocator.AnnouncementService.GetAnnouncementById(announcementId);

                var student = ServiceLocator.StudentService.GetById(x.StudentId, Context.SchoolYearId.Value);
                var details = new StudentDetails
                {
                    BirthDate = student.BirthDate,
                    FirstName = student.FirstName,
                    Gender = student.Gender,
                    HasMedicalAlert = student.HasMedicalAlert,
                    Id = student.Id,
                    IsAllowedInetAccess = student.IsAllowedInetAccess,
                    IsWithdrawn = null,
                    LastName = student.LastName,
                    PhotoModifiedDate = null,
                    SpecialInstructions = student.SpecialInstructions,
                    SpEdStatus = student.SpEdStatus,
                    UserId = student.UserId
                };

                return new StudentAnnouncementDetails
                {
                    AnnouncementId = x.AnnouncementId,
                    AbsenceCategory = x.AbsenceCategory,
                    Absent = x.Absent,
                    ActivityId = x.ActivityId,
                    AlphaGradeId = x.AlphaGradeId,
                    AlternateScoreId = x.AlternateScoreId,
                    Comment = x.Comment,
                    ScoreDropped = x.ScoreDropped,
                    Exempt = x.Exempt,
                    ExtraCredit = x.ExtraCredit,
                    Incomplete = x.Incomplete,
                    ClassId = ann.ClassRef,
                    Late = x.Late,
                    NumericScore = x.NumericScore,
                    OverMaxScore = x.OverMaxScore,
                    ScoreValue = x.ScoreValue,
                    Student = details,
                    StudentId = x.StudentId,
                    Withdrawn = x.Withdrawn
                };

            }).ToList();

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
                    scores.Add(ActivityScoreStorage.GetScore(ann.SisActivityId.Value, Context.PersonId.Value));
                    persons.Add(ServiceLocator.StudentService.GetById(Context.PersonId.Value, Context.SchoolYearId.Value));
                }
                else
                {
                    scores = ActivityScoreStorage.GetSores(ann.SisActivityId.Value);
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
            var annApp = ServiceLocator.AnnouncementAttachmentService.GetAnnouncementApplication(announcementApplicationId);
            var autoGrade = StorageLocator.AutoGradeStorage.GetAutoGrade(announcementApplicationId, recepientId);

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
                StorageLocator.AutoGradeStorage.Add(autoGrade);
            }
            else
            {
                autoGrade.Grade = value;
                autoGrade.Date = Context.NowSchoolYearTime;
                StorageLocator.AutoGradeStorage.SetAutoGrade(autoGrade);
            }
            return autoGrade;

        }

        public IList<AutoGrade> GetAutoGradesByAnnouncementId(int announcementId)
        {
            return ServiceLocator.AutoGradeStorage.GetAutoGradesByAnnouncementId(announcementId);
        }

        public IList<AutoGrade> GetAutoGrades(int announcementApplicationId)
        {
            return StorageLocator.AutoGradeStorage.GetAutoGrades(announcementApplicationId);
        }
    }
}
