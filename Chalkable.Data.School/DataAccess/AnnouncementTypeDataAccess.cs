using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementTypeDataAccess : DataAccessBase<AnnouncementType, int>
    {
        public AnnouncementTypeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<AnnouncementType> GetList(bool? gradeble)
        {
            var conds = new AndQueryCondition();
            if (gradeble.HasValue)
                conds.Add(AnnouncementType.GRADABLE_FIELD_NAME, gradeble.Value);
            return SelectMany<AnnouncementType>(conds);
        }
    }

    public class ClassAnnouncementTypeDataAccess : DataAccessBase<ClassAnnouncementType, int>
    {
        public ClassAnnouncementTypeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
