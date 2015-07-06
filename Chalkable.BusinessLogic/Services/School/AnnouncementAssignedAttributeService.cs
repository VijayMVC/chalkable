using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAnnouncementAssignedAttributeService
    {
        void Add(IList<AnnouncementAssignedAttribute> announcementAttributes);
        AnnouncementDetails Edit(int announcementType, int announcementId, IList<AnnouncementAssignedAttribute> announcementAttributes);
        void Delete(IList<AnnouncementAssignedAttribute> announcementAttributes);
        AnnouncementDetails Delete(int announcementType, int announcementId, int assignedAttributeId);
        AnnouncementDetails Add(int announcementType, int announcementId, int attributeTypeId);
    }

    public class AnnouncementAssignedAttributeService : SchoolServiceBase, IAnnouncementAssignedAttributeService
    {
        public AnnouncementAssignedAttributeService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        public void Add(IList<AnnouncementAssignedAttribute> announcementAttributes)
        {
            BaseSecurity.EnsureAdminOrTeacher(Context);
            DoUpdate(u => new DataAccessBase<AnnouncementAssignedAttribute>(u).Insert(announcementAttributes));
        }

        public AnnouncementDetails Edit(int announcementType, int announcementId, IList<AnnouncementAssignedAttribute> announcementAttributes)
        {
            BaseSecurity.EnsureAdminOrTeacher(Context);
            var ann = ServiceLocator.GetAnnouncementService((AnnouncementType)announcementType).GetAnnouncementDetails(announcementId);
            if (!(Context.PersonId.HasValue && Context.SchoolLocalId.HasValue))
                throw new UnassignedUserException();

            if (announcementAttributes != null)
            {
                using (var uow = Update())
                {
                    var da = new DataAccessBase<AnnouncementAssignedAttribute, int>(uow);

                    da.Update(announcementAttributes);
                    uow.Commit();
                    ann.AnnouncementAttributes = da.GetAll();
                }
            }
            
            return ann;
        }

        public void Delete(IList<AnnouncementAssignedAttribute> announcementAttributes)
        {
            BaseSecurity.EnsureAdminOrTeacher(Context);
            DoUpdate(u => new DataAccessBase<AnnouncementAssignedAttribute>(u).Delete(announcementAttributes));
        }

        public AnnouncementDetails Delete(int announcementType, int announcementId, int assignedAttributeId)
        {
            BaseSecurity.EnsureAdminOrTeacher(Context);
            var ann = ServiceLocator.GetAnnouncementService((AnnouncementType)announcementType).GetAnnouncementDetails(announcementId);
            if (!(Context.PersonId.HasValue && Context.SchoolLocalId.HasValue))
                throw new UnassignedUserException();

            using (var uow = Update())
            {
                var da = new DataAccessBase<AnnouncementAssignedAttribute, int>(uow);

                da.Delete(assignedAttributeId);
                uow.Commit();
                ann.AnnouncementAttributes = da.GetAll();
            }
            return ann;
        }

        private bool CanAttach(Announcement ann)
        {
            return AnnouncementSecurity.CanModifyAnnouncement(ann, Context);
        }

        public AnnouncementDetails Add(int announcementType, int announcementId, int attributeTypeId)
        {
            var ann = ServiceLocator.GetAnnouncementService((AnnouncementType)announcementType).GetAnnouncementDetails(announcementId);
            if (!(Context.PersonId.HasValue && Context.SchoolLocalId.HasValue))
                throw new UnassignedUserException();

            var attributeType = ServiceLocator.AnnouncementAttributeService.GetAttributeById(attributeTypeId, true);

            using (var uow = Update())
            {
                if (!CanAttach(ann))
                    throw new ChalkableSecurityException();

                var annAttribute = new AnnouncementAssignedAttribute
                {
                    AnnouncementRef = ann.Id,
                    AttributeTypeId = attributeType.Id,
                    Name = attributeType.Name,
                    Text = " ",
                    VisibleForStudents = true
                };
                var da = new DataAccessBase<AnnouncementAssignedAttribute,int>(uow);
                da.Insert(annAttribute);
                uow.Commit();
                ann.AnnouncementAttributes = da.GetAll();
            }
            return ann;
        }
    }
}
