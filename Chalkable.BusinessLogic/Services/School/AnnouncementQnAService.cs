using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        //IList<AnnouncementQnAComplex> GetAnnouncementQnAsComplex(int announcementId);
        AnnouncementQnA GetAnnouncmentQnA(Guid announcementQnAId);
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
            //TODO : security
            using (var uow = Update())
            {
                var da = new AnnouncementQnADataAccess(uow);
                var annQnA = new AnnouncementQnA
                    {
                        Id = Guid.NewGuid(),
                        AnnouncementRef = announcementId,
                        PersonRef = Context.UserId,
                        Question = question,
                        QuestionTime = Context.NowSchoolTime,
                        State = AnnouncementQnAState.Asked

                    };
                da.Create(annQnA);
                //TODO : send asked ann notification to teacher
                uow.Commit();
                return annQnA;
            }
        }

        public AnnouncementQnA Answer(Guid announcementQnAId, string question, string answer)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementQnADataAccess(uow);
                var annQnA = da.GetWithAnnouncement(announcementQnAId);
                if(!AnnouncementSecurity.CanModifyAnnouncement(annQnA.Announcement, Context))
                    throw new ChalkableSecurityException();

                annQnA.State = AnnouncementQnAState.Answered;
                annQnA.Question = question;
                annQnA.Answer = answer;
                annQnA.AnsweredTime = Context.NowSchoolTime;
                da.Update(annQnA);
                //TODO : send answer ann notification to student
                uow.Commit();
                return annQnA;
            }
        }

        public AnnouncementQnA EditAnswer(Guid announcementQnAId, string question)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementQnADataAccess(uow);
                var annQnA = da.GetWithAnnouncement(announcementQnAId);
                if (!AnnouncementSecurity.CanModifyAnnouncement(annQnA.Announcement, Context))
                    throw new ChalkableSecurityException();

                annQnA.Question = question;
                annQnA.QuestionTime = Context.NowSchoolTime;
                da.Update(annQnA);
                uow.Commit();
                return annQnA;
            }
        }

        public AnnouncementQnA EditQuestion(Guid announcementQnAId, string answer)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementQnADataAccess(uow);
                var annQnA = da.GetWithAnnouncement(announcementQnAId);
                if (!AnnouncementSecurity.CanModifyAnnouncement(annQnA.Announcement, Context))
                    throw new ChalkableSecurityException();

                annQnA.Answer = answer;
                annQnA.AnsweredTime = Context.NowSchoolTime;
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
                var guidId = announcementQnAId;
                var annQnA = da.GetWithAnnouncement(guidId);
                if (!AnnouncementSecurity.CanModifyAnnouncement(annQnA.Announcement, Context))
                    throw new ChalkableSecurityException();

                da.Delete(annQnA);
                uow.Commit();
            }
        }

        public AnnouncementQnA MarkUnanswered(Guid announcementQnAId)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementQnADataAccess(uow);
                var annQnA = da.GetWithAnnouncement(announcementQnAId);
                if (!AnnouncementSecurity.CanModifyAnnouncement(annQnA.Announcement, Context))
                    throw new ChalkableSecurityException();

                annQnA.State = AnnouncementQnAState.Unanswered;
                da.Update(annQnA);
                uow.Commit();
                return annQnA;
            }
        }

        public AnnouncementQnA GetAnnouncmentQnA(Guid announcementQnAId)
        {
            //TODO : security 
            using (var uow = Read())
            {
                var da = new AnnouncementQnADataAccess(uow);
                return da.GetById(announcementQnAId);
            }
        }
    }
}
