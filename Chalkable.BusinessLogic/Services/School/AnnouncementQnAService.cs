using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{

    public interface IAnnouncementQnAService
    {
        AnnouncementQnA AskQuestion(int announcementId, string question);
        AnnouncementQnA Answer(int announcementQnAId, string question, string answer);
        AnnouncementQnA EditAnswer(int announcementQnAId, string question);
        AnnouncementQnA EditQuestion(int announcementQnAId, string answer);
        void Delete(int announcementQnAId);
        AnnouncementQnA MarkUnanswered(int announcementQnAId);
        IList<AnnouncementQnAComplex> GetAnnouncementQnAs(int announcementId);
        AnnouncementQnAComplex GetAnnouncementQnA(int announcementQnAId);
    }


    public class AnnouncementQnAService :SchoolServiceBase, IAnnouncementQnAService
    {
        public AnnouncementQnAService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }


        //TODO : notification sending 
        //TODO : tests 

        public AnnouncementQnA AskQuestion(int announcementId, string question)
        {
            using (var uow = Update())
            {
                if (!(Context.PersonId.HasValue && Context.SchoolLocalId.HasValue))
                    throw new UnassignedUserException();
          
                var da = new AnnouncementQnADataAccess(uow);
                var ann = ServiceLocator.AnnouncementService.GetAnnouncementById(announcementId);
                
                var annQnA = new AnnouncementQnAComplex
                    {
                        AnnouncementRef = announcementId,
                        AskerRef = Context.PersonId.Value,
                        Question = question,
                        QuestionTime = Context.NowSchoolTime,
                        State = AnnouncementQnAState.Asked
                    };
                da.Insert(annQnA);
                uow.Commit();
                annQnA = da.GetAnnouncementQnA(new AnnouncementQnAQuery 
                                {
                                    AnnouncementId = announcementId,
                                    CallerId = annQnA.AskerRef,
                                    SchoolId = Context.SchoolLocalId.Value
                                }).AnnouncementQnAs.OrderByDescending(x=>x.Id).First();
                ServiceLocator.NotificationService.AddAnnouncementNotificationQnToAuthor(annQnA.Id, ann.Id);
                return annQnA;
            }
        }

        public AnnouncementQnA Answer(int announcementQnAId, string question, string answer)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            using (var uow = Update())
            {
                var da = new AnnouncementQnADataAccess(uow);
                var annQnA = GetAnnouncementQnA(announcementQnAId);
                // todo: think about security
                if (!CanEditQuestion(annQnA, uow))
                    throw new ChalkableSecurityException();

                annQnA.State = AnnouncementQnAState.Answered;
                annQnA.Question = question;
                if (Context.Role == CoreRoles.TEACHER_ROLE && (!annQnA.AnswererRef.HasValue || annQnA.AnswererRef == Context.PersonId))
                {
                    var answerer = new PersonDataAccess(uow, Context.SchoolLocalId).GetById(Context.PersonId.Value);
                    annQnA.Answerer = answerer;
                    annQnA.AnswererRef = answerer.Id;
                    annQnA.AnsweredTime = Context.NowSchoolTime;
                    annQnA.Answer = answer;
                }
                da.Update(annQnA);
                uow.Commit();
                var ann = ServiceLocator.AnnouncementService.GetAnnouncementById(annQnA.AnnouncementRef);
                if(ann.VisibleForStudent)
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
                if (!CanEditAnswer(annQnA, uow))
                    throw new ChalkableSecurityException();

                annQnA.Answer = answer;
                da.Update(annQnA);
                uow.Commit();
                return annQnA;
            }
        }

        private bool CanEditAnswer(AnnouncementQnAComplex announcementQnA, UnitOfWork uow)
        {
            var da = new ClassTeacherDataAccess(uow);
            return BaseSecurity.IsSysAdmin(Context) || announcementQnA.AnswererRef == Context.PersonId
                || (Context.PersonId.HasValue && Context.Role == CoreRoles.TEACHER_ROLE && string.IsNullOrEmpty(announcementQnA.Answer)
                       && da.Exists(announcementQnA.ClassRef, Context.PersonId.Value));
        }

        private bool CanEditQuestion(AnnouncementQnAComplex announcementQnA, UnitOfWork uow)
        {
            var da = new ClassTeacherDataAccess(uow);
            return BaseSecurity.IsSysAdmin(Context) || announcementQnA.AskerRef == Context.PersonId
                || (Context.PersonId.HasValue && Context.Role == CoreRoles.TEACHER_ROLE
                    && da.Exists(announcementQnA.ClassRef, Context.PersonId.Value));
        }

        public AnnouncementQnA EditQuestion(int announcementQnAId, string question)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementQnADataAccess(uow);
                var annQnA = GetAnnouncementQnA(announcementQnAId);
                if (!CanEditQuestion(annQnA, uow))
                    throw new ChalkableSecurityException();
                
                annQnA.Question = question;
                da.Update(annQnA);
                uow.Commit();
                return annQnA;
            }
        }

        public void Delete(int announcementQnAId)
        {
            // todo: think about security
         
            using (var uow = Update())
            {
                var da = new AnnouncementQnADataAccess(uow);
                var annQnA = GetAnnouncementQnA(announcementQnAId);
                //if (!CanEditAnswer(annQnA, uow))
                //    throw new ChalkableSecurityException();

                da.Delete(annQnA.Id);
                uow.Commit();
            }
        }

        public AnnouncementQnA MarkUnanswered(int announcementQnAId)
        {
            // todo: think about security
         
            using (var uow = Update())
            {
                var da = new AnnouncementQnADataAccess(uow);
                var annQnA = GetAnnouncementQnA(announcementQnAId);
                //if (!CanEditAnswer(annQnA, uow))
                //    throw new ChalkableSecurityException();

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
            if (!(Context.PersonId.HasValue && Context.SchoolLocalId.HasValue))
                throw new UnassignedUserException();

            query.CallerId = Context.PersonId;
            query.SchoolId = Context.SchoolLocalId.Value;
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
