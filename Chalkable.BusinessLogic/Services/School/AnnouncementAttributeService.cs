using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAnnouncementAttributeService
    {
        void Add(IList<AnnouncementAttribute> announcementAttributes);
        void Edit(IList<AnnouncementAttribute> announcementAttributes);
        void Delete(IList<AnnouncementAttribute> announcementAttributes);
        IList<AnnouncementAttribute> GetList(bool? activeOnly);
        AnnouncementAttribute GetAttributeById(int attributeId, bool? activeOnly);
    }

    public class AnnouncementAttributeService : SchoolServiceBase, IAnnouncementAttributeService
    {
        public AnnouncementAttributeService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<AnnouncementAttribute> announcementAttributes)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<AnnouncementAttribute>(u).Insert(announcementAttributes));
        }

        public void Edit(IList<AnnouncementAttribute> announcementAttributes)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<AnnouncementAttribute>(u).Update(announcementAttributes));
        }

        public void Delete(IList<AnnouncementAttribute> announcementAttributes)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new AnnouncementAttributeDataAccess(u).Delete(announcementAttributes));
        }

        public IList<AnnouncementAttribute> GetList(bool? activeOnly)
        {
            AndQueryCondition conds = null;
            if (activeOnly.HasValue && activeOnly.Value)
                conds = new AndQueryCondition {{AnnouncementAttribute.IS_ACTIVE_FIELD, true}};
            return DoRead(u => new DataAccessBase<AnnouncementAttribute>(u).GetAll(conds));
        }

        public AnnouncementAttribute GetAttributeById(int attributeId, bool? activeOnly)
        {
            var conds = new AndQueryCondition
            {
                {AnnouncementAttribute.ID_FIELD, true}
            };

            if (activeOnly.HasValue && activeOnly.Value)
                conds = new AndQueryCondition
                {
                    {AnnouncementAttribute.ID_FIELD, true},
                    { AnnouncementAttribute.IS_ACTIVE_FIELD, true }
                };

            return DoRead(u => new DataAccessBase<AnnouncementAttribute>(u).GetAll(conds).First());
        }
    }
}
