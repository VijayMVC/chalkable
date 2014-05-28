using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class ClassAnnouncementTypeDataAccess : DataAccessBase<ClassAnnouncementType, int>
    {
        public ClassAnnouncementTypeDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public void Delete(IList<int> ids)
        {
            SimpleDelete<ClassAnnouncementType>(ids.Select(x => new ClassAnnouncementType {Id = x}).ToList());
        }
    }
}