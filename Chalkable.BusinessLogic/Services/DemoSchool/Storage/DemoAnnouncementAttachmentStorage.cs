﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{

    class AnnouncementAttachmentQuery
    {
        public int? AnnouncementId { get; set; }    
        public int? Start { get; set; }
        public int? Count { get; set; }
        public int? CallerId { get; set; }
        public int? RoleId { get; set; }
        public bool NeedsAllAttachments { get; set; }
        public string Filter { get; set; }
        public string Name { get; set; }
        public int? Id { get; set; }
    }

    public class DemoAnnouncementAttachmentStorage:BaseDemoIntStorage<AnnouncementAttachment>
    {
        public DemoAnnouncementAttachmentStorage(DemoStorage storage) : base(storage, x => x.Id, true)
        {
        }


        private IList<AnnouncementAttachment> GetAttachmentsQuery(AnnouncementAttachmentQuery query)
        {
            query.CallerId = Storage.Context.PersonId;
            query.RoleId = Storage.Context.Role.Id;

            var attachments = data.Select(x => x.Value);

            if (query.AnnouncementId.HasValue)
            {
                attachments = attachments.Where(x => x.AnnouncementRef == query.AnnouncementId);
            }

            if (CoreRoles.SUPER_ADMIN_ROLE.Id == query.RoleId)
            {
                return attachments.ToList();
            }
            if (CoreRoles.ADMIN_EDIT_ROLE.Id == query.RoleId || CoreRoles.ADMIN_GRADE_ROLE.Id == query.RoleId ||
                CoreRoles.ADMIN_VIEW_ROLE.Id == query.RoleId)
            {

                attachments = attachments.Where(x => x.PersonRef == query.CallerId);
                return attachments.ToList();
            }
            if (CoreRoles.TEACHER_ROLE.Id == query.RoleId)
            {

                var announcementRefs =
                    Storage.AnnouncementRecipientStorage.GetAll()
                        .Where(x => x.RoleRef == query.RoleId || x.PersonRef == query.CallerId)
                        .Select(x => x.AnnouncementRef);

                attachments =
                    attachments.Where(
                        x => 
                        {
                            var announcement = Storage.AnnouncementStorage.GetById(x.AnnouncementRef);
                            return x.PersonRef == query.CallerId ||x.PersonRef == announcement.PrimaryTeacherRef || announcementRefs.Contains(x.AnnouncementRef);
                        });

            }
            if (CoreRoles.STUDENT_ROLE.Id == query.RoleId)
            {
                var classRefs = Storage.ClassPersonStorage.GetAll().Where( x => x.PersonRef == query.CallerId).Select(x => x.ClassRef).ToList();

                attachments = attachments.Where(x =>
                {
                    var classRef = Storage.AnnouncementStorage.GetById(x.AnnouncementRef).ClassRef;
                    return (x.PersonRef == query.CallerId || classRefs.Contains(classRef));
                });
                attachments.ToList();
            }


            if (!query.NeedsAllAttachments)
                attachments = attachments.Where(x => x.PersonRef == query.CallerId);

            if (!string.IsNullOrEmpty(query.Filter))
            {
                var filters =
                    query.Filter.Trim()
                        .Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.ToLower())
                        .ToList();

                attachments = attachments.Where(x => filters.Contains(x.Name.ToLower()));
            }

            if (query.Start.HasValue)
                attachments = attachments.Skip(query.Start.Value);
            if (query.Count.HasValue)
                attachments = attachments.Take(query.Count.Value);
            return attachments.ToList();
        } 

        public IList<AnnouncementAttachment> GetList(int userId, int roleId, string name)
        {
            return GetAttachmentsQuery(new AnnouncementAttachmentQuery
            {
                CallerId = userId,
                RoleId = roleId,
                Name = name
            });
        }


        public PaginatedList<AnnouncementAttachment> GetPaginatedList(int announcementId, int userId, int roleId, int start, int count, bool needsAllAttachments)
        {

            var attachments = GetAttachmentsQuery(new AnnouncementAttachmentQuery
            {
                AnnouncementId = announcementId,
                Start = start,
                Count = count,
                NeedsAllAttachments = needsAllAttachments,
                CallerId = userId,
                RoleId = roleId
            });
            return new PaginatedList<AnnouncementAttachment>(attachments, start / count, count, data.Count);
        }

       


        public AnnouncementAttachment GetById(int announcementAttachmentId, int userId, int roleId)
        {
            return GetAttachmentsQuery(new AnnouncementAttachmentQuery
            {
                Id = announcementAttachmentId,
                CallerId = userId,
                RoleId = roleId
            }).First();
        }

        public IList<AnnouncementAttachment> GetAll(int announcementId)
        {
            return GetAttachmentsQuery(new AnnouncementAttachmentQuery
            {
                AnnouncementId = announcementId
            });
        }
    }
}
