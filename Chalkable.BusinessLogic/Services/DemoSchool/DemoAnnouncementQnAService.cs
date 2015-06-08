using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoAnnouncementQnAStorage : BaseDemoIntStorage<AnnouncementQnAComplex>
    {
        public DemoAnnouncementQnAStorage()
            : base(x => x.Id, true)
        {
        }
    }

    public class DemoAnnouncementQnAService :DemoSchoolServiceBase, IAnnouncementQnAService
    {
        private DemoAnnouncementQnAStorage AnnouncementQnAStorage { get; set; }
        public DemoAnnouncementQnAService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            AnnouncementQnAStorage = new DemoAnnouncementQnAStorage();
        }

        public AnnouncementQnAQueryResult GetAnnouncementQnA(AnnouncementQnAQuery announcementQnAQuery)
        {
            var qnas = AnnouncementQnAStorage.GetData().Select(x => x.Value);

            if (announcementQnAQuery.Id.HasValue)
                qnas = qnas.Where(x => x.Id == announcementQnAQuery.Id);
            if (announcementQnAQuery.AskerId.HasValue)
                qnas = qnas.Where(x => x.AskerRef == announcementQnAQuery.AskerId);


            if (announcementQnAQuery.AnnouncementId.HasValue)
                qnas = qnas.Where(x => x.AnnouncementRef == announcementQnAQuery.AnnouncementId);

            if (announcementQnAQuery.AnswererId.HasValue)
            {
                var announcementQnAComplexs = qnas as IList<AnnouncementQnAComplex> ?? qnas.ToList();
                var announcementIds = announcementQnAComplexs.Select(x => x.AnnouncementRef);
                var personIds = announcementIds.Select(annId => ServiceLocator.AnnouncementService.GetAnnouncementById(annId))
                    .Select(announcement => announcement.PrimaryTeacherRef).ToList();
                qnas = announcementQnAComplexs.Where(x => personIds.Contains(x.AskerRef));
            }

            return new AnnouncementQnAQueryResult
            {
                AnnouncementQnAs = qnas.ToList(),
                Query = announcementQnAQuery
            };
        }

        public AnnouncementQnA AskQuestion(int announcementId, string question)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();

            var ann = ServiceLocator.AnnouncementService.GetAnnouncementById(announcementId);

            if(!ann.PrimaryTeacherRef.HasValue)
                throw new ChalkableException("There is no teachers for that item");

            var annQnA = new AnnouncementQnAComplex
            {
                AnnouncementRef = announcementId,
                AskerRef = Context.PersonId.Value,
                ClassRef = ann.ClassRef.Value,
                Question = question,
                QuestionTime = Context.NowSchoolTime,
                State = AnnouncementQnAState.Asked,
                Asker = ServiceLocator.PersonService.GetPersonDetails(Context.PersonId.Value),
                Answerer = ServiceLocator.PersonService.GetPersonDetails(ann.PrimaryTeacherRef.Value)
            };
            AnnouncementQnAStorage.Add(annQnA);
            annQnA = GetAnnouncementQnA(new AnnouncementQnAQuery
            {
                AnnouncementId = announcementId,
                CallerId = annQnA.AskerRef
            }).AnnouncementQnAs.OrderByDescending(x => x.Id).First();
            ServiceLocator.NotificationService.AddAnnouncementNotificationQnToTeacher(annQnA.Id, ann.Id);
            return annQnA;
        }

        private bool CanEditAnswer(AnnouncementQnAComplex announcementQnA)
        {
            return BaseSecurity.IsSysAdmin(Context) || announcementQnA.AnswererRef == Context.PersonId
                || (Context.PersonId.HasValue && Context.Role == CoreRoles.TEACHER_ROLE && string.IsNullOrEmpty(announcementQnA.Answer)
                       && ((DemoClassService)ServiceLocator.ClassService).ClassTeacherExists(announcementQnA.ClassRef, Context.SchoolLocalId.Value));
        }

        private bool CanEditQuestion(AnnouncementQnAComplex announcementQnA)
        {
            return BaseSecurity.IsSysAdmin(Context) || announcementQnA.AskerRef == Context.PersonId
                || (Context.PersonId.HasValue && Context.Role == CoreRoles.TEACHER_ROLE
                    && ((DemoClassService)ServiceLocator.ClassService).ClassTeacherExists(announcementQnA.ClassRef, Context.PersonId.Value));
        }

        public AnnouncementQnA Answer(int announcementQnAId, string question, string answer)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();

            var annQnA = GetAnnouncementQnA(announcementQnAId);
                if (!CanEditQuestion(annQnA))
                    throw new ChalkableSecurityException();

            annQnA.State = AnnouncementQnAState.Answered;
            annQnA.Question = question;
            if (Context.Role == CoreRoles.TEACHER_ROLE && (!annQnA.AnswererRef.HasValue || annQnA.AnswererRef == Context.PersonId))
            {
                var answerer = ServiceLocator.PersonService.GetPersonDetails(Context.PersonId.Value);
                annQnA.Answerer = answerer;
                annQnA.AnswererRef = answerer.Id;
                annQnA.AnsweredTime = Context.NowSchoolTime;
                annQnA.Answer = answer;
            }
        
            AnnouncementQnAStorage.Update(annQnA);
            ServiceLocator.NotificationService.AddAnnouncementNotificationAnswerToStudent(annQnA.Id, annQnA.AnnouncementRef);
            return annQnA;
        }

        public AnnouncementQnA EditAnswer(int announcementQnAId, string answer)
        {
            var annQnA = GetAnnouncementQnA(announcementQnAId);
            if (!CanEditAnswer(annQnA))
                throw new ChalkableSecurityException();

            annQnA.Answer = answer;
            annQnA.AnsweredTime = Context.NowSchoolTime;
            AnnouncementQnAStorage.Update(annQnA);
            return annQnA;
        }

        public AnnouncementQnA EditQuestion(int announcementQnAId, string question)
        {
            var annQnA = GetAnnouncementQnA(announcementQnAId);
            if (!CanEditAnswer(annQnA))
                throw new ChalkableSecurityException();

            annQnA.Question = question;
            annQnA.QuestionTime = Context.NowSchoolTime;
            AnnouncementQnAStorage.Update(annQnA);
            return annQnA;
        }

        public void Delete(int announcementQnAId)
        {
            var annQnA = GetAnnouncementQnA(announcementQnAId);
            AnnouncementQnAStorage.Delete(annQnA.Id);
        }

        public AnnouncementQnA MarkUnanswered(int announcementQnAId)
        {
            var annQnA = GetAnnouncementQnA(announcementQnAId);
            annQnA.State = AnnouncementQnAState.Unanswered;
            AnnouncementQnAStorage.Update(annQnA);
            return annQnA;
        }

        public AnnouncementQnAComplex GetAnnouncementQnA(int announcementQnAId)
        {
            return GetAnnouncmentQnAs(new AnnouncementQnAQuery {Id = announcementQnAId}).AnnouncementQnAs.First();
        }

        private AnnouncementQnAQueryResult GetAnnouncmentQnAs(AnnouncementQnAQuery query)
        {
            query.CallerId = Context.PersonId;
            return GetAnnouncementQnA(query);
        }

        public IList<AnnouncementQnAComplex> GetAnnouncementQnAs(int announcementId)
        {
            var query = new AnnouncementQnAQuery {AnnouncementId = announcementId};
            return GetAnnouncmentQnAs(query).AnnouncementQnAs;
        }

        public void AddQnA(AnnouncementQnAComplex announcementQnAComplex)
        {
            AnnouncementQnAStorage.Add(announcementQnAComplex);
        }
    }
}
