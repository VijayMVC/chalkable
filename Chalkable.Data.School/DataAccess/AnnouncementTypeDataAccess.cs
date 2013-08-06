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
        public IList<AnnouncementType> GetList(bool? gradeble)
        {
            var conds = new Dictionary<string, object>();
            if (gradeble.HasValue)
                conds.Add(AnnouncementType.GRADABLE_FIELD_NAME, gradeble.Value);
            return SelectMany<AnnouncementType>(conds);
        }
    }
}
