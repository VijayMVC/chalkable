﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAnnouncementAttachmentStorage:BaseDemoStorage<int, AnnouncementAttachment>
    {
        public DemoAnnouncementAttachmentStorage(DemoStorage storage) : base(storage)
        {
        }

        public void Add(AnnouncementAttachment annAtt)
        {
            if (!data.ContainsKey(annAtt.Id))
                data[annAtt.Id] = annAtt;
        }

        public IList<AnnouncementAttachment> GetList(int userId, int roleId, string name)
        {

            //todo filter by role id
            return data.Where(x => x.Value.PersonRef == userId && x.Value.Name == name).Select(x => x.Value).ToList();
        }

        public PaginatedList<AnnouncementAttachment> GetPaginatedList(int announcementId, int start = 0, int count = int.MaxValue, bool needsAllAttachments = true)
        {
            var attachments =
                data.Where(x => x.Value.AnnouncementRef == announcementId)
                    .Skip(start)
                    .Take(count)
                    .Select(x => x.Value);
            return new PaginatedList<AnnouncementAttachment>(attachments, start / count, count, data.Count);
        }

        /*private DbQuery BuildGetAttachmentQuery(QueryConditionSet queryCondition, int callerId, int roleId, bool needsAllAttachments = true, string filter = null)
        {
            var res = new DbQuery();
            var type = typeof(AnnouncementAttachment);
            res.Sql.AppendFormat(@"select [{0}].* from [{0}] 
                                   join [{2}] on [{2}].[{3}] = [{0}].[{1}]"
                              , type.Name, AnnouncementAttachment.ANNOUNCEMENT_REF_FIELD, "Announcement", Announcement.ID_FIELD);

            queryCondition.BuildSqlWhere(res, type.Name);
            if (!needsAllAttachments)
                res.Sql.AppendFormat(" and AnnouncementAttachment.PersonRef = @callerId");
            
            if (!string.IsNullOrEmpty(filter))
            {
                string[] sl = filter.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var filters = new List<string>();
                if (sl.Length > 0)
                {
                    filters.Add("@filter1");
                    res.Parameters.Add("@filter1", string.Format(FILTER_FORMAT, sl[0]));
                }
                if (sl.Length > 1)
                {
                    filters.Add("@filter2");
                    res.Parameters.Add("@filter2", string.Format(FILTER_FORMAT, sl[1]));
                }
                if (sl.Length > 2)
                {
                    filters.Add("@filter3");
                    res.Parameters.Add("@filter3", string.Format(FILTER_FORMAT, sl[2]));
                }
                res.Sql.AppendFormat(" and (LOWER(Name) like {0})", filters.JoinString(" or LOWER(Name) like "));
            }

            res.Parameters.Add("@callerId", callerId);
            res.Parameters.Add("@roleId", roleId);
            if (CoreRoles.SUPER_ADMIN_ROLE.Id == roleId)
            {
                return res;
            }
            if (CoreRoles.ADMIN_EDIT_ROLE.Id == roleId || CoreRoles.ADMIN_GRADE_ROLE.Id == roleId ||
                CoreRoles.ADMIN_VIEW_ROLE.Id == roleId)
            {
                res.Sql.Append("and AnnouncementAttachment.PersonRef =@callerId");
                return res;
            }
            if (CoreRoles.TEACHER_ROLE.Id == roleId)
            {
                res.Sql.Append(@" and (Announcement.PersonRef = @callerId or AnnouncementAttachment.PersonRef = Announcement.PersonRef 
                                    or (Announcement.Id in (select AnnouncementRef from AnnouncementRecipient 
                                                           where RoleRef = @roleId or PersonRef = @callerId or ToAll = 1) 
                                         and AnnouncementAttachment.PersonRef = Announcement.PersonRef)
                                 )");
                return res;

            }
            if (CoreRoles.STUDENT_ROLE.Id == roleId)
            {
                res.Sql.Append(@" and (AnnouncementAttachment.PersonRef = @callerId 
                                   or (Announcement.ClassRef in (select cp.ClassRef from ClassPerson cp where cp.PersonRef = @callerId)
                                       and AnnouncementAttachment.PersonRef = Announcement.PersonRef)
                                )");
                return res;
            }
            return null;
        }*/


        public AnnouncementAttachment GetById(int announcementAttachmentId, int userId, int roleId)
        {
            //todo filter by role
            return
                data.First(x => x.Value.Id == announcementAttachmentId && x.Value.PersonRef == userId).Value;

        }

        public IList<AnnouncementAttachment> GetAll(int announcementId)
        {
            return data.Where(x => x.Value.AnnouncementRef == announcementId).Select(x => x.Value).ToList();
        }
    }
}
