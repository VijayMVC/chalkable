using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{

    public class DemoStudentAnnouncementService : DemoSchoolServiceBase, IStudentAnnouncementService
    {
        public DemoStudentAnnouncementService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
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

            decimal numericScore = 0;
            if (gradingStyle.HasValue)
            {
                if (gradingStyle.Value != GradingStyleEnum.Numeric100)
                {
                    var numScore = Storage.AlphaGradeStorage.GetAll().FirstOrDefault(x => x.Name.ToLowerInvariant() == value);
                    if (numScore != null)
                    {
                        var gs =
                            Storage.GradingScaleRangeStorage.GetAll().FirstOrDefault(x => x.AlphaGradeRef == numScore.Id);

                        if (gs != null)
                        {
                            numericScore = gs.AveragingEquivalent;
                        }
                    }
                }
                else
                {
                    decimal.TryParse(value, out numericScore);
                }
            }

            var stAnn = new StudentAnnouncement
            {
                ExtraCredit = extraCredits,
                Comment = comment != null && !string.IsNullOrWhiteSpace(comment) ? comment.Trim() : "",
                Dropped = dropped,
                Incomplete = incomplete,
                Late = late,
                Exempt = exempt,
                ScoreValue = numericScore.ToString(CultureInfo.InvariantCulture),
                NumericScore = numericScore,
                
                ActivityId = ann.SisActivityId.Value,
                AnnouncementId = announcementId,
                StudentId = studentId
            };
            var score = new Score();
            MapperFactory.GetMapper<Score, StudentAnnouncement>().Map(score, stAnn);
            score = Storage.StiActivityScoreStorage.UpdateScore(score.ActivityId, score.StudentId, score);
            score.ActivityDate = ann.Expires;
            score.ActivityName = ann.Title;
            MapperFactory.GetMapper<StudentAnnouncement, Score>().Map(stAnn, score);

            if (stAnn.AlternateScoreId.HasValue)
                stAnn.AlternateScore = ServiceLocator.AlternateScoreService.GetAlternateScore(stAnn.AlternateScoreId.Value);
            if (ann.VisibleForStudent && !string.IsNullOrWhiteSpace(value))
                ServiceLocator.NotificationService.AddAnnouncementSetGradeNotificationToPerson(announcementId, stAnn.StudentId);
            return stAnn;
        }

        public IList<StudentAnnouncementDetails> GetStudentAnnouncements(int announcementId)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            var ann = ServiceLocator.AnnouncementService.GetAnnouncementById(announcementId);
            if (ann.SisActivityId.HasValue)
            {
                IList<Score> scores = new List<Score>();
                IList<Person> persons = new List<Person>();
                if (CoreRoles.STUDENT_ROLE == Context.Role)
                {
                    scores.Add(Storage.StiActivityScoreStorage.GetScore(ann.SisActivityId.Value, Context.PersonId.Value));
                    persons.Add(ServiceLocator.PersonService.GetPerson(Context.PersonId.Value));
                }
                else
                {
                    scores = Storage.StiActivityScoreStorage.GetSores(ann.SisActivityId.Value);
                    persons = ServiceLocator.PersonService.GetPaginatedPersons(new PersonQuery { ClassId = ann.ClassRef });
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

        //TODO: check application existing  
        public StudentAnnouncement SetAutoGrade(int studentAnnouncementId, int value, Guid applicationId)
        {
            throw new NotImplementedException();
        }


        public void ResolveAutoGrading(int announcementId, bool apply)
        {
            throw new NotImplementedException();
        }
    }
}
