using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{

    public interface IAnnouncementQnAService
    {
        AnnouncementQnA AskQuestion(Guid announcementId, string question);
        AnnouncementQnA Answer(Guid announcementQnAId, string question, string answer);

        AnnouncementQnA EditAnswer(Guid announcementQnAId, string question);
        AnnouncementQnA EditQuestion(Guid announcementQnAId, string answer);
        void Delete(Guid announcementQnAId);
        AnnouncementQnA MarkUnanswered(Guid announcementQnAId);
        IList<AnnouncementQnAComplex> GetAnnouncementQnAs(Guid announcementId);
        AnnouncementQnAComplex GetAnnouncementQnA(Guid announcementQnAId);
    }


    public class AnnouncementQnAService :SchoolServiceBase, IAnnouncementQnAService
    {
        public AnnouncementQnAService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        //TODO : notification sending 
        //TODO : tests 

        public AnnouncementQnA AskQuestion(Guid announcementId, string question)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementQnADataAccess(uow);
                var ann = ServiceLocator.AnnouncementService.GetAnnouncementById(announcementId);
                
                var annQnA = new AnnouncementQnAComplex
                    {
                        Id = Guid.NewGuid(),
                        AnnouncementRef = announcementId,
                        PersonRef = Context.UserId,
                        Question = question,
                        QuestionTime = Context.NowSchoolTime,
                        State = AnnouncementQnAState.Asked
                    };
                da.Insert(annQnA);
                uow.Commit();
                ServiceLocator.NotificationService.AddAnnouncementNotificationQnToAuthor(annQnA.Id, ann.Id);
                return annQnA;
            }
        }

        public AnnouncementQnA Answer(Guid announcementQnAId, string question, string answer)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementQnADataAccess(uow);
                var annQnA = GetAnnouncementQnA(announcementQnAId);
                if (!AnnouncementSecurity.CanModifyAnnouncementQnA(annQnA, Context))
                    throw new ChalkableSecurityException();

                annQnA.State = AnnouncementQnAState.Answered;
                annQnA.Question = question;
                annQnA.Answer = answer;
                annQnA.AnsweredTime = Context.NowSchoolTime;
                da.Update(annQnA);
                uow.Commit();
                ServiceLocator.NotificationService.AddAnnouncementNotificationAnswerToPerson(annQnA.Id, annQnA.AnnouncementRef);
                return annQnA;
            }
        }

        public AnnouncementQnA EditAnswer(Guid announcementQnAId, string answer)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementQnADataAccess(uow);
                var annQnA = GetAnnouncementQnA(announcementQnAId);
                if (!AnnouncementSecurity.CanModifyAnnouncementQnA(annQnA, Context))
                    throw new ChalkableSecurityException();

                annQnA.Answer = answer;
                annQnA.AnsweredTime = Context.NowSchoolTime;
                da.Update(annQnA);
                uow.Commit();
                return annQnA;
            }
        }

        public AnnouncementQnA EditQuestion(Guid announcementQnAId, string question)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementQnADataAccess(uow);
                var annQnA = GetAnnouncementQnA(announcementQnAId);
                if (!AnnouncementSecurity.CanModifyAnnouncementQnA(annQnA, Context))
                    throw new ChalkableSecurityException();

                
                annQnA.Question = question;
                annQnA.QuestionTime = Context.NowSchoolTime;
                da.Update(annQnA);
                uow.Commit();
                return annQnA;
            }
        }

        public void Delete(Guid announcementQnAId)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementQnADataAccess(uow);
                var annQnA = GetAnnouncementQnA(announcementQnAId);
                if (!AnnouncementSecurity.CanModifyAnnouncementQnA(annQnA, Context))
                    throw new ChalkableSecurityException();

                da.Delete(annQnA.Id);
                uow.Commit();
            }
        }

        public AnnouncementQnA MarkUnanswered(Guid announcementQnAId)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementQnADataAccess(uow);
                var annQnA = GetAnnouncementQnA(announcementQnAId);
                if (!AnnouncementSecurity.CanModifyAnnouncementQnA(annQnA, Context))
                    throw new ChalkableSecurityException();

                annQnA.State = AnnouncementQnAState.Unanswered;
                da.Update(annQnA);
                uow.Commit();
                return annQnA;
            }
        }

        public AnnouncementQnAComplex GetAnnouncementQnA(Guid announcementQnAId)
        {
            return GetAnnouncmentQnAs(new AnnouncementQnAQuery {Id = announcementQnAId}).AnnouncementQnAs.First();
        }

        private AnnouncementQnAQueryResult GetAnnouncmentQnAs(AnnouncementQnAQuery query)
        {
            query.CallerId = Context.UserId;
            using (var uow = Read())
            {
                return new AnnouncementQnADataAccess(uow).GetAnnouncementQnA(query);
            }
        }

        public IList<AnnouncementQnAComplex> GetAnnouncementQnAs(Guid announcementId)
        {
            var query = new AnnouncementQnAQuery {AnnouncementId = announcementId};
            return GetAnnouncmentQnAs(query).AnnouncementQnAs;
        }
    }
}
