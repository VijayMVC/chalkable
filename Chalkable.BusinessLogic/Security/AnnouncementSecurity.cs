﻿using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Security
{
    public static class AnnouncementSecurity
    {
        public static bool CanCreateAnnouncement(UserContext context)
        {
            return BaseSecurity.IsDistrictAdmin(context) || context.Role  == CoreRoles.TEACHER_ROLE;
        }

        public static bool CanModifyAnnouncement(Announcement announcement, UserContext context)
        {
            return BaseSecurity.IsSysAdmin(context) || announcement.IsOwner;
        }

        public static void EnsureInModifyAccess(Announcement announcement, UserContext context)
        {
            if(!CanModifyAnnouncement(announcement, context))
                throw new ChalkableSecurityException();
        }

        public static bool CanDeleteAnnouncement(Announcement announcement, UserContext context)
        {
            return BaseSecurity.IsSysAdmin(context) || announcement.IsOwner;
        }

        public static bool CanDeleteAnnouncement(int ownerId, UserContext context)
        {
            return BaseSecurity.IsSysAdmin(context) || context.PersonId == ownerId;
        }

        public static bool CanDeleteAttachment(AnnouncementAttachment announcementAttachment, UserContext context)
        {
            return BaseSecurity.IsSysAdmin(context) || announcementAttachment.PersonRef == context.PersonId;
        }

    }
}
