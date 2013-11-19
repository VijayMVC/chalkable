using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IClassAnnouncementTypeService
    {
        IList<ClassAnnouncementType> GetClassAnnouncementTypes(int classId, bool all = true);
        void Add(IList<ClassAnnouncementType> classAnnouncementTypes);
    }

    public class ClassClassAnnouncementTypeService : SchoolServiceBase, IClassAnnouncementTypeService
    {
        public ClassClassAnnouncementTypeService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<ClassAnnouncementType> GetClassAnnouncementTypes(int classId, bool all = true)
        {
            using (var uow = Read())
            {
                var cond = new AndQueryCondition{{ClassAnnouncementType.CLASS_REF_FIELD, classId}};
                var res = new ClassAnnouncementTypeDataAccess(uow).GetAll(cond);
                if (!all)
                    res = res.Where(x => x.Percentage > 0).ToList();
                return res;
            }
        }

        public void Add(IList<ClassAnnouncementType> classAnnouncementTypes)
        {
            using (var uow = Update())
            {
                var da = new ClassAnnouncementTypeDataAccess(uow);
                da.Insert(classAnnouncementTypes);
                uow.Commit();
            }
        }
    }
}
