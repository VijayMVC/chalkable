using System.Collections.Generic;
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

        //TODO : notification sending 
        //TODO : tests 

        public AnnouncementQnA AskQuestion(int announcementId, string question)
        {
            using (var uow = Update())
            {
                if (!(Context.UserLocalId.HasValue && Context.SchoolId.HasValue))
                    throw new UnassignedUserException();
          
                var da = new AnnouncementQnADataAccess(uow);
                var ann = ServiceLocator.AnnouncementService.GetAnnouncementById(announcementId);
                
                var annQnA = new AnnouncementQnAComplex
                    {
                        AnnouncementRef = announcementId,
                        PersonRef = Context.UserLocalId.Value,
                        Question = question,
                        QuestionTime = Context.NowSchoolTime,
                        State = AnnouncementQnAState.Asked
                    };
                da.Insert(annQnA);
                uow.Commit();
                annQnA = da.GetAnnouncementQnA(new AnnouncementQnAQuery 
                                {
                                    AnnouncementId = announcementId,
                                    CallerId = annQnA.PersonRef
                                }).AnnouncementQnAs.OrderByDescending(x=>x.Id).First();
                ServiceLocator.NotificationService.AddAnnouncementNotificationQnToAuthor(annQnA.Id, ann.Id);
                return annQnA;
            }
        }

        public AnnouncementQnA Answer(int announcementQnAId, string question, string answer)
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

        public AnnouncementQnA EditAnswer(int announcementQnAId, string answer)
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

        public AnnouncementQnA EditQuestion(int announcementQnAId, string question)
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

        public void Delete(int announcementQnAId)
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

        public AnnouncementQnA MarkUnanswered(int announcementQnAId)
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

        public AnnouncementQnAComplex GetAnnouncementQnA(int announcementQnAId)
        {
            return GetAnnouncmentQnAs(new AnnouncementQnAQuery {Id = announcementQnAId}).AnnouncementQnAs.First();
        }

        private AnnouncementQnAQueryResult GetAnnouncmentQnAs(AnnouncementQnAQuery query)
        {
            query.CallerId = Context.UserLocalId;
            using (var uow = Read())
            {
                return new AnnouncementQnADataAccess(uow).GetAnnouncementQnA(query);
            }
        }

        public IList<AnnouncementQnAComplex> GetAnnouncementQnAs(int announcementId)
        {
            var query = new AnnouncementQnAQuery {AnnouncementId = announcementId};
            return GetAnnouncmentQnAs(query).AnnouncementQnAs;
        }
    }
}
