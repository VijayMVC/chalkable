using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoAnnouncementQnAService :DemoSchoolServiceBase, IAnnouncementQnAService
    {
        public DemoAnnouncementQnAService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator ,storage)
        {
        }

        public AnnouncementQnA AskQuestion(int announcementId, string question)
        {
            if (!(Context.UserLocalId.HasValue && Context.SchoolId.HasValue))
                throw new UnassignedUserException();

            var ann = ServiceLocator.AnnouncementService.GetAnnouncementById(announcementId);

            if(!ann.PrimaryTeacherRef.HasValue)
                throw new ChalkableException("There is no teachers for that item");

            var annQnA = new AnnouncementQnAComplex
            {
                AnnouncementRef = announcementId,
                AskerRef = Context.UserLocalId.Value,
                ClassRef = ann.ClassRef,
                Question = question,
                QuestionTime = Context.NowSchoolTime,
                State = AnnouncementQnAState.Asked,
                Asker = Storage.PersonStorage.GetById(Context.UserLocalId.Value),
                Answerer = Storage.PersonStorage.GetById(ann.PrimaryTeacherRef.Value)
            };
            Storage.AnnouncementQnAStorage.Add(annQnA);
            annQnA = Storage.AnnouncementQnAStorage.GetAnnouncementQnA(new AnnouncementQnAQuery
            {
                AnnouncementId = announcementId,
                CallerId = annQnA.AskerRef
            }).AnnouncementQnAs.OrderByDescending(x => x.Id).First();
            ServiceLocator.NotificationService.AddAnnouncementNotificationQnToAuthor(annQnA.Id, ann.Id);
            return annQnA;
        }

        private bool CanEditAnswer(AnnouncementQnAComplex announcementQnA)
        {
            return BaseSecurity.IsSysAdmin(Context) || announcementQnA.AnswererRef == Context.UserLocalId
                || (Context.UserLocalId.HasValue && Context.Role == CoreRoles.TEACHER_ROLE && string.IsNullOrEmpty(announcementQnA.Answer)
                       && Storage.ClassTeacherStorage.Exists(announcementQnA.ClassRef, Context.SchoolLocalId.Value));
        }

        private bool CanEditQuestion(AnnouncementQnAComplex announcementQnA)
        {
            return BaseSecurity.IsSysAdmin(Context) || announcementQnA.AskerRef == Context.UserLocalId
                || (Context.UserLocalId.HasValue && Context.Role == CoreRoles.TEACHER_ROLE
                    && Storage.ClassTeacherStorage.Exists(announcementQnA.ClassRef, Context.UserLocalId.Value));
        }

        public AnnouncementQnA Answer(int announcementQnAId, string question, string answer)
        {
            if (!Context.UserLocalId.HasValue)
                throw new UnassignedUserException();

            var annQnA = GetAnnouncementQnA(announcementQnAId);
            // todo: think about security
                if (!CanEditQuestion(annQnA))
                    throw new ChalkableSecurityException();

            annQnA.State = AnnouncementQnAState.Answered;
            annQnA.Question = question;
            if (Context.Role == CoreRoles.TEACHER_ROLE && (!annQnA.AnswererRef.HasValue || annQnA.AnswererRef == Context.UserLocalId))
            {
                var answerer = Storage.PersonStorage.GetById(Context.UserLocalId.Value);
                annQnA.Answerer = answerer;
                annQnA.AnswererRef = answerer.Id;
                annQnA.AnsweredTime = Context.NowSchoolTime;
                annQnA.Answer = answer;
            }
        
            Storage.AnnouncementQnAStorage.Update(annQnA);
            ServiceLocator.NotificationService.AddAnnouncementNotificationAnswerToPerson(annQnA.Id, annQnA.AnnouncementRef);
            return annQnA;
        }

        public AnnouncementQnA EditAnswer(int announcementQnAId, string answer)
        {
            var annQnA = GetAnnouncementQnA(announcementQnAId);
            if (!CanEditAnswer(annQnA))
                throw new ChalkableSecurityException();

            annQnA.Answer = answer;
            annQnA.AnsweredTime = Context.NowSchoolTime;
            Storage.AnnouncementQnAStorage.Update(annQnA);
            return annQnA;
        }

        public AnnouncementQnA EditQuestion(int announcementQnAId, string question)
        {
            var annQnA = GetAnnouncementQnA(announcementQnAId);
            if (!CanEditAnswer(annQnA))
                throw new ChalkableSecurityException();

            annQnA.Question = question;
            annQnA.QuestionTime = Context.NowSchoolTime;
            Storage.AnnouncementQnAStorage.Update(annQnA);
            return annQnA;
        }

        public void Delete(int announcementQnAId)
        {
            var annQnA = GetAnnouncementQnA(announcementQnAId);
            Storage.AnnouncementQnAStorage.Delete(annQnA.Id);
        }

        public AnnouncementQnA MarkUnanswered(int announcementQnAId)
        {
            var annQnA = GetAnnouncementQnA(announcementQnAId);
            annQnA.State = AnnouncementQnAState.Unanswered;
            Storage.AnnouncementQnAStorage.Update(annQnA);
            return annQnA;
        }

        public AnnouncementQnAComplex GetAnnouncementQnA(int announcementQnAId)
        {
            return GetAnnouncmentQnAs(new AnnouncementQnAQuery {Id = announcementQnAId}).AnnouncementQnAs.First();
        }

        private AnnouncementQnAQueryResult GetAnnouncmentQnAs(AnnouncementQnAQuery query)
        {
            query.CallerId = Context.UserLocalId;
            return Storage.AnnouncementQnAStorage.GetAnnouncementQnA(query);
        }

        public IList<AnnouncementQnAComplex> GetAnnouncementQnAs(int announcementId)
        {
            var query = new AnnouncementQnAQuery {AnnouncementId = announcementId};
            return GetAnnouncmentQnAs(query).AnnouncementQnAs;
        }
    }
}
