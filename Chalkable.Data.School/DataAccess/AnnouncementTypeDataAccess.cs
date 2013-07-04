using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementTypeDataAccess : DataAccessBase<AnnouncementType>
    {
        public AnnouncementTypeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public AnnouncementType GetById(int id)
        {
            return SelectOne<AnnouncementType>(new Dictionary<string, object> {{"Id", id}});
        }
    }
}
