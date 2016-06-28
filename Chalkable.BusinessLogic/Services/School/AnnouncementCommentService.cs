using System;
using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAnnouncementCommentService
    {
        IList<AnnouncementComment> GetList(int announcementId); 
        AnnouncementComment PostComment(int announcementId, string text, int? attachmentId);
        AnnouncementComment Reply(int toCommentId, string text, int? attachmentId);
        void SetHidden(int commentId, bool hidden);
        void Delete(int commnetId);
    }
    public class AnnouncementCommentService : SchoolServiceBase, IAnnouncementCommentService
    {
        public AnnouncementCommentService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<AnnouncementComment> GetList(int announcementId)
        {
            throw new NotImplementedException();
        }

        public AnnouncementComment PostComment(int announcementId, string text, int? attachmentId)
        {
            throw new NotImplementedException();
        }
        public AnnouncementComment Reply(int toCommentId, string text, int? attachmentId)
        {
            throw new NotImplementedException();
        }
        public void SetHidden(int commentId, bool hidden)
        {
            throw new NotImplementedException();
        }
        public void Delete(int commnetId)
        {
            throw new NotImplementedException();
        }
    }
}
