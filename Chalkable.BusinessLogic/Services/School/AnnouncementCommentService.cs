using System.Collections.Generic;
using System.Diagnostics;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAnnouncementCommentService
    {
        IList<AnnouncementComment> GetList(int announcementId); 
        AnnouncementComment PostComment(int announcementId, string text, IList<int> attachmentIds);
        AnnouncementComment Reply(int toCommentId, string text, IList<int> attachmentIds);
        AnnouncementComment Edit(int announcementCommentId, string text, IList<int> attachmentIds);
        void SetHidden(int commentId, bool hidden);
        void Delete(int commnetId);
        AnnouncementComment GetById(int announcementCommentId);
    }
    public class AnnouncementCommentService : SchoolServiceBase, IAnnouncementCommentService
    {
        public AnnouncementCommentService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<AnnouncementComment> GetList(int announcementId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => new AnnouncementCommentDataAccess(u).GetCommentsTree(announcementId, Context.PersonId.Value, Context.RoleId));
        }

        public AnnouncementComment PostComment(int announcementId, string text, IList<int> attachmentIds)
        {
            var type = ServiceLocator.AnnouncementFetchService.GetAnnouncementType(announcementId);
            var ann = ServiceLocator.GetAnnouncementService(type).GetAnnouncementById(announcementId);
            EnsureInCreateAccess(ann);
            
            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Update())
            {
                var da =  new AnnouncementCommentDataAccess(uow);
                var id = da.InsertWithEntityId(new AnnouncementComment
                    {
                        AnnouncementRef = announcementId,
                        PersonRef = Context.PersonId.Value,
                        Text = text,
                        PostedDate = Context.NowSchoolYearTime,
                        Hidden = ann.PreviewCommentsEnabled
                    });
                new AnnouncementCommentAttachmentDataAccess(uow).PostAttachements(id, attachmentIds);
                uow.Commit();
                return da.GetDetailsById(id, Context.PersonId.Value, Context.RoleId);
            }
        }
        
        public AnnouncementComment Reply(int toCommentId, string text, IList<int> attachmentIds)
        {
            Trace.Assert(Context.PersonId.HasValue);
            
            using (var uow = Update())
            {
                var da = new AnnouncementCommentDataAccess(uow);
                var parentComment = da.GetById(toCommentId);
                var type = ServiceLocator.AnnouncementFetchService.GetAnnouncementType(parentComment.AnnouncementRef);
                var ann = ServiceLocator.GetAnnouncementService(type).GetAnnouncementById(parentComment.AnnouncementRef);
                EnsureInCreateAccess(ann);

                var id = da.InsertWithEntityId(new AnnouncementComment
                {
                    AnnouncementRef = parentComment.AnnouncementRef,
                    PersonRef = Context.PersonId.Value,
                    Text = text,
                    PostedDate = Context.NowSchoolTime,
                    ParentCommentRef = toCommentId,
                    Hidden = ann.PreviewCommentsEnabled
                });

                new AnnouncementCommentAttachmentDataAccess(uow).PostAttachements(id, attachmentIds);
                uow.Commit();
                return da.GetDetailsById(id, Context.PersonId.Value, Context.RoleId);
            }
        }
        public AnnouncementComment Edit(int announcementCommentId, string text, IList<int> attachmentIds)
        {
            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Update())
            {
                var da = new AnnouncementCommentDataAccess(uow);
                var comment = da.GetDetailsById(announcementCommentId, Context.PersonId.Value, Context.RoleId);
                EnsureInCreateAccess(comment.AnnouncementRef);
                if (comment.PersonRef != Context.PersonId)
                    throw  new ChalkableSecurityException("Only owner can edit comment");

                comment.Text = text;
                comment.PostedDate = Context.NowSchoolTime;
                da.Update(comment);

                new AnnouncementCommentAttachmentDataAccess(uow).PostAttachements(announcementCommentId, attachmentIds);
                uow.Commit();
                return da.GetDetailsById(announcementCommentId, Context.PersonId.Value, Context.RoleId);
            }
        }

        
        public void SetHidden(int commentId, bool hidden)
        {
            BaseSecurity.EnsureAdminOrTeacher(Context);
            var comment = GetById(commentId);
            EnsureInDeleteAccess(comment);

            comment.Hidden = hidden;
            var toUpdate = new List<AnnouncementComment> {comment};
            var allSubComments = comment.AllSubComments;
            if (allSubComments != null)
                foreach (var subComment in allSubComments)
                {
                    subComment.Hidden = true;
                    toUpdate.Add(subComment);
                }
            DoUpdate(u=> new AnnouncementCommentDataAccess(u).Update(toUpdate));
        }
        public void Delete(int commentId)
        {
            BaseSecurity.EnsureAdminOrTeacher(Context);
            var comment = GetById(commentId);
            EnsureInDeleteAccess(comment);

            comment.Deleted = true;
            var toUpdate = new List<AnnouncementComment> { comment };
            var allSubComments = comment.AllSubComments;
            if (allSubComments != null)
                foreach (var subComment in allSubComments)
                {
                    subComment.Deleted = true;
                    toUpdate.Add(subComment);
                }
            DoUpdate(u => new AnnouncementCommentDataAccess(u).Update(toUpdate));
        }

        public AnnouncementComment GetById(int commentId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => new AnnouncementCommentDataAccess(u).GetDetailsById(commentId, Context.PersonId.Value, Context.RoleId));
        }


        private void EnsureInCreateAccess(int announcementId, AnnouncementTypeEnum? type = null)
        {
            type = type ?? ServiceLocator.AnnouncementFetchService.GetAnnouncementType(announcementId);
            var ann = ServiceLocator.GetAnnouncementService(type).GetAnnouncementById(announcementId);
            EnsureInCreateAccess(ann);
        }

        private static void EnsureInCreateAccess(Announcement announcement)
        {
            if (announcement.IsDraft)
                throw new ChalkableException("Item is not submited yet");
            if (!announcement.DiscussionEnabled)
                throw new ChalkableException("Discussion option is disabled for current item");
        }
        
        private void EnsureInDeleteAccess(AnnouncementComment comment, AnnouncementTypeEnum? type = null)
        {
            EnsureInCreateAccess(comment.AnnouncementRef, type);
            if(!BaseSecurity.IsDistrictOrTeacher(Context))
                throw new ChalkableSecurityException();
        }
    }
}
