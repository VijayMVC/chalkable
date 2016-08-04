using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementAttributeDataAccess : DataAccessBase<AnnouncementAttribute, int>
    {
        public AnnouncementAttributeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override void Delete(IList<AnnouncementAttribute> entities)
        {
            var parameters = new Dictionary<string, object>
            {
                {"attributeIds", entities.Select(x => x.Id).ToList()}
            };
            ExecuteStoredProcedure("spDeleteAnnouncementAttribute", parameters);
        }
    }
}
