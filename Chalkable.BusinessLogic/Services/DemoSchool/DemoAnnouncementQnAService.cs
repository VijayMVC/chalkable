﻿using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
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

            var annQnA = new AnnouncementQnAComplex
            {
                AnnouncementRef = announcementId,
                PersonRef = Context.UserLocalId.Value,
                Question = question,
                QuestionTime = Context.NowSchoolTime,
                State = AnnouncementQnAState.Asked
            };
            Storage.AnnouncementQnAStorage.Add(annQnA);
            annQnA = Storage.AnnouncementQnAStorage.GetAnnouncementQnA(new AnnouncementQnAQuery
            {
                AnnouncementId = announcementId,
                CallerId = annQnA.PersonRef
            }).AnnouncementQnAs.OrderByDescending(x => x.Id).First();
            ServiceLocator.NotificationService.AddAnnouncementNotificationQnToAuthor(annQnA.Id, ann.Id);
            return annQnA;
        }

        public AnnouncementQnA Answer(int announcementQnAId, string question, string answer)
        {
            var annQnA = GetAnnouncementQnA(announcementQnAId);
            if (!AnnouncementSecurity.CanModifyAnnouncementQnA(annQnA, Context))
                throw new ChalkableSecurityException();

            annQnA.State = AnnouncementQnAState.Answered;
            annQnA.Question = question;
            annQnA.Answer = answer;
            annQnA.AnsweredTime = Context.NowSchoolTime;
            Storage.AnnouncementQnAStorage.Update(annQnA);
            ServiceLocator.NotificationService.AddAnnouncementNotificationAnswerToPerson(annQnA.Id, annQnA.AnnouncementRef);
            return annQnA;
        }

        public AnnouncementQnA EditAnswer(int announcementQnAId, string answer)
        {
            var annQnA = GetAnnouncementQnA(announcementQnAId);
            if (!AnnouncementSecurity.CanModifyAnnouncementQnA(annQnA, Context))
                throw new ChalkableSecurityException();

            annQnA.Answer = answer;
            annQnA.AnsweredTime = Context.NowSchoolTime;
            Storage.AnnouncementQnAStorage.Update(annQnA);
            return annQnA;
        }

        public AnnouncementQnA EditQuestion(int announcementQnAId, string question)
        {
            var annQnA = GetAnnouncementQnA(announcementQnAId);
            if (!AnnouncementSecurity.CanModifyAnnouncementQnA(annQnA, Context))
                throw new ChalkableSecurityException();


            annQnA.Question = question;
            annQnA.QuestionTime = Context.NowSchoolTime;
            Storage.AnnouncementQnAStorage.Update(annQnA);
            return annQnA;
        }

        public void Delete(int announcementQnAId)
        {
            var annQnA = GetAnnouncementQnA(announcementQnAId);
            if (!AnnouncementSecurity.CanModifyAnnouncementQnA(annQnA, Context))
                throw new ChalkableSecurityException();
            Storage.AnnouncementQnAStorage.Delete(annQnA.Id);
        }

        public AnnouncementQnA MarkUnanswered(int announcementQnAId)
        {
            var annQnA = GetAnnouncementQnA(announcementQnAId);
            if (!AnnouncementSecurity.CanModifyAnnouncementQnA(annQnA, Context))
                throw new ChalkableSecurityException();

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
