using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoActivityScoreStorage : BaseDemoIntStorage<Score>
    {
        public DemoActivityScoreStorage()
            : base(null, true)
        {
        }


        public Score GetScore(int sisActivityId, int userId)
        {
            return data.First(x => x.Value.ActivityId == sisActivityId && x.Value.StudentId == userId).Value;
        }

        public IList<Score> GetScores(int userId)
        {
            return data.Where(x => x.Value.StudentId == userId).Select(x => x.Value).ToList();
        }

        public IList<Score> GetSores(int sisActivityId)
        {
            return data.Where(x => x.Value.ActivityId == sisActivityId).Select(x => x.Value).ToList();
        }


        public void DuplicateFrom(int fromSisActivityId, int toSisActivityId)
        {
            var srcScores = GetScores(fromSisActivityId);
            var resScores = srcScores.Select(srcScore => new Score
            {
                AbsenceCategory = srcScore.AbsenceCategory,
                Absent = srcScore.Absent,
                ActivityDate = srcScore.ActivityDate,
                ActivityId = toSisActivityId,
                AlphaGradeId = srcScore.AlphaGradeId,
                AlternateScoreId = srcScore.AlternateScoreId,
                Comment = srcScore.Comment,
                Exempt = srcScore.Exempt,
                Dropped = srcScore.Dropped,
                Incomplete = srcScore.Incomplete,
                Late = srcScore.Late,
                NumericScore = srcScore.NumericScore,
                OverMaxScore = srcScore.OverMaxScore,
                ScoreValue = srcScore.ScoreValue,
                StudentId = srcScore.StudentId,
                Withdrawn = srcScore.Withdrawn
            }).ToList();
            Add(resScores);
        }

        public Score UpdateScore(int activityId, int studentId, Score score)
        {
            var item = data.First(x => x.Value.ActivityId == activityId && x.Value.StudentId == studentId);
            data[item.Key] = score;
            return score;
        }

        public void ResetScores(int sisActivityId, IEnumerable<int> studentIds)
        {
            foreach (var studentId in studentIds)
            {
                UpdateScore(sisActivityId, studentId, new Score()
                {
                    ActivityId = sisActivityId,
                    StudentId = studentId
                });
            }
        }
    }

    public class DemoAutoGradeStorage : BaseDemoIntStorage<AutoGrade>
    {
        public DemoAutoGradeStorage()
            : base(null, true)
        {
        }


        public void SetAutoGrade(AutoGrade autograde)
        {
            var item = data.First(
                 x => x.Value.AnnouncementApplicationRef == autograde.AnnouncementApplicationRef && 
                     autograde.StudentRef == x.Value.StudentRef);

            data[item.Key] = autograde;
        }

        public AutoGrade GetAutoGrade(int announcementApplicationId, int? recipientId = null)
        {
            return GetAll().FirstOrDefault(x => x.AnnouncementApplicationRef == announcementApplicationId
                                                        && (!recipientId.HasValue || x.StudentRef == recipientId));
        }

        public IList<AutoGrade> GetAutoGrades(int announcementApplicationId)
        {
            return GetAll().Where(x => x.AnnouncementApplicationRef == announcementApplicationId).ToList();
        }
    }

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
        private DemoActivityScoreStorage ActivityScoreStorage { get; set; }
        private DemoAutoGradeStorage AutoGradeStorage { get; set; }
        public DemoStudentAnnouncementService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            StudentAnnouncementStorage = new DemoStudentAnnouncementStorage();
            ActivityScoreStorage = new DemoActivityScoreStorage();
            AutoGradeStorage = new DemoAutoGradeStorage();
        }

        public IList<AutoGrade> GetAutoGradesByAnnouncementId(int announcementId)
        {
            var annApps = ServiceLocator.ApplicationSchoolService.GetAnnouncementApplicationsByAnnId(announcementId);
            return AutoGradeStorage.GetAll()
                       .Where(x => annApps.Any(y => y.Id == x.AnnouncementApplicationRef))
                       .ToList();
        }

        public void AddStudentAnnouncement(StudentAnnouncement studentAnnouncement)
        {
            StudentAnnouncementStorage.Add(studentAnnouncement);
            var score = new Score();
            MapperFactory.GetMapper<Score, StudentAnnouncement>().Map(score, studentAnnouncement);
            ActivityScoreStorage.Add(score);
        }

        public Task<IList<StudentAnnouncementDetails>> GetStudentAnnouncementsByAnnIds(IList<int> announcementIds, int studentId)
        {
            throw new NotImplementedException();
        }

        public StudentAnnouncement SetGrade(int announcementId, int studentId, string value, string extraCredits,
            string comment, bool dropped, bool late, bool exempt, bool incomplete, GradingStyleEnum? gradingStyle = null)
        {
            var ann = ServiceLocator.ClassAnnouncementService.GetClassAnnouncemenById(announcementId);

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
                    var gradingScaleRange =
                        ((DemoGradingScaleService) ServiceLocator.GradingScaleService).GetByAlphaGradeId(numScore.Id);
                    if (gradingScaleRange != null)
                    {
                        numericScore = gradingScaleRange.AveragingEquivalent;
                        value = numScore.Name;
                    }
                }
            }

            var gradeComment = comment != null && !string.IsNullOrWhiteSpace(comment) ? comment.Trim() : "";
            var oldScore = ActivityScoreStorage.GetScore(ann.SisActivityId.Value, studentId);
            var studentAnnouncement = new StudentAnnouncement();
            MapperFactory.GetMapper<StudentAnnouncement, Score>().Map(studentAnnouncement, oldScore);
            studentAnnouncement.AnnouncementId = oldScore.ActivityId;

            if (numericScore >= 0 && value != null)
            {
                studentAnnouncement.NumericScore = numericScore;
                studentAnnouncement.ScoreValue = isDecimal
                    ? string.Format("{0:0.00}", numericScore)
                    : value.ToString(CultureInfo.InvariantCulture);
            }
            else if (value == null)
            {
                studentAnnouncement.NumericScore = null;
                studentAnnouncement.ScoreValue = "";
            }

            if (value == null && oldScore.ScoreValue != null)
            {
                studentAnnouncement.NumericScore = null;
                studentAnnouncement.ScoreValue = "";
            }
            studentAnnouncement.Exempt = exempt;
            studentAnnouncement.Late = late;
            studentAnnouncement.Incomplete = incomplete;
            studentAnnouncement.ExtraCredit = extraCredits;

            if (studentAnnouncement.NumericScore.HasValue || studentAnnouncement.Late ||
                studentAnnouncement.Incomplete)
            {
                studentAnnouncement.ScoreDropped = dropped;
                studentAnnouncement.Comment = gradeComment;
            }
            else
            {
                studentAnnouncement.ScoreDropped = false;
            }
            var score = new Score();
            MapperFactory.GetMapper<Score, StudentAnnouncement>().Map(score, studentAnnouncement);
            score = ActivityScoreStorage.UpdateScore(score.ActivityId, score.StudentId, score);
            score.ActivityDate = ann.Expires;
            score.ActivityName = ann.Title;
            MapperFactory.GetMapper<StudentAnnouncement, Score>().Map(studentAnnouncement, score);

            if (studentAnnouncement.AlternateScoreId.HasValue)
                studentAnnouncement.AlternateScore = ServiceLocator.AlternateScoreService.GetAlternateScore(studentAnnouncement.AlternateScoreId.Value);
            if (ann.VisibleForStudent)
                ServiceLocator.NotificationService.AddAnnouncementSetGradeNotificationToStudent(announcementId, studentAnnouncement.StudentId);
            return studentAnnouncement;
        }

        public IList<StudentAnnouncementDetails> GetAll(int announcementId)
        {
            return StudentAnnouncementStorage.GetAll().Where(x => x.AnnouncementId == announcementId).Select(x =>
            {
                var ann = ServiceLocator.ClassAnnouncementService.GetClassAnnouncemenById(announcementId);

                var student = ServiceLocator.StudentService.GetById(x.StudentId, Context.SchoolYearId.Value);
                var details = new Student
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


        public IList<Score> GetActivityScores(IList<int> activityIds)
        {
            return ActivityScoreStorage.GetAll().Where(x => activityIds.Contains(x.ActivityId)).ToList();
        }

        public IList<Score> GetActivityScoresForStudent(int studentId)
        {
            return ActivityScoreStorage.GetAll().Where(x => x.StudentId == studentId).ToList();
        }

        public IList<StudentAnnouncementDetails> GetStudentAnnouncements(int announcementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);
            var ann = ServiceLocator.ClassAnnouncementService.GetClassAnnouncemenById(announcementId);
            var mp = ServiceLocator.MarkingPeriodService.GetMarkingPeriodByDate(ann.Expires, true);
            Trace.Assert(mp != null);
            if (ann.SisActivityId.HasValue)
            {
                IList<Score> scores = new List<Score>();
                IList<Student> persons = new List<Student>();
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
            var annApp = ServiceLocator.ApplicationSchoolService.GetAnnouncementApplication(announcementApplicationId);
            var autoGrade = AutoGradeStorage.GetAutoGrade(announcementApplicationId, recepientId);

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
                AutoGradeStorage.Add(autoGrade);
            }
            else
            {
                autoGrade.Grade = value;
                autoGrade.Date = Context.NowSchoolYearTime;
                AutoGradeStorage.SetAutoGrade(autoGrade);
            }
            return autoGrade;

        }

        public IList<AutoGrade> GetAutoGrades(int announcementApplicationId)
        {
            return AutoGradeStorage.GetAutoGrades(announcementApplicationId);
        }

        public void DropUndropAnnouncement(int announcementId, bool drop)
        {
            StudentAnnouncementStorage.Update(announcementId, drop);
        }

        public void DeleteStudentAnnouncements(int announcementId, int sisActivityId)
        {
            var scores = ActivityScoreStorage.GetSores(sisActivityId);
            ActivityScoreStorage.Delete(scores);
            var studentAnnouncements = StudentAnnouncementStorage.GetAll()
                    .Where(x => x.AnnouncementId == announcementId)
                    .ToList();
            StudentAnnouncementStorage.Delete(studentAnnouncements);
        }

        public void ResetScores(int announcementId, int sisActivityId)
        {
            var studentIds = GetStudentAnnouncements(announcementId).Select(x => x.StudentId);
            ActivityScoreStorage.ResetScores(sisActivityId, studentIds);
        }
    }
}
