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
    }
}