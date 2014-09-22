using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Security
{
    public static class AnnouncementSecurity
    {
        public static bool CanCreateAnnouncement(UserContext context)
        {
            return BaseSecurity.IsAdminViewer(context) || context.Role  == CoreRoles.TEACHER_ROLE;
        }

        public static bool CanModifyAnnouncement(Announcement announcement, UserContext context)
        {
            return BaseSecurity.IsSysAdmin(context) || announcement.IsOwner;
        }

        public static bool CanDeleteAnnouncement(Announcement announcement, UserContext context)
        {
            return BaseSecurity.IsSysAdmin(context) || announcement.IsOwner;
        }


       

        //public static bool CanModifyAnnouncementQnA(AnnouncementQnAComplex announcementQnA, UserContext context)
        //{
        //    return BaseSecurity.IsSysAdmin(context) || context.UserLocalId == announcementQnA.Answerer.Id;
        //}

        public static bool CanDeleteAttachment(AnnouncementAttachment announcementAttachment, UserContext context)
        {
            return BaseSecurity.IsSysAdmin(context) || announcementAttachment.PersonRef == context.PersonId;
        }
        //public static bool CanAttach(AnnouncementDetails announcementDetails, UserContext context)
        //{
        //    return CanModifyAnnouncement(announcementDetails, context) ||
        //           announcementDetails.StudentAnnouncements.Any(x => x.Student.Id == context.UserLocalId);
            
        //}
    }
}
