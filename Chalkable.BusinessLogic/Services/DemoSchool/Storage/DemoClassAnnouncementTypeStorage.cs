using System.Collections.Generic;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoClassAnnouncementTypeStorage:BaseDemoStorage<int, ClassAnnouncementType>
    {
        public DemoClassAnnouncementTypeStorage(DemoStorage storage) : base(storage)
        {
        }

        public IList<ClassAnnouncementType> GetAll(AndQueryCondition cond)
        {
            throw new System.NotImplementedException();
        }

        public void Add(IList<ClassAnnouncementType> classAnnouncementTypes)
        {
            throw new System.NotImplementedException();
        }

        public void Edit(IList<ClassAnnouncementType> classAnnouncementTypes)
        {
            throw new System.NotImplementedException();
        }
    }
}
