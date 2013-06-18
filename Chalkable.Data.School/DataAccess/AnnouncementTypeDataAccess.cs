using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementTypeDataAccess : DataAccessBase
    {
        public AnnouncementTypeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public AnnouncementType GetById(int id)
        {
            var conds = new Dictionary<string, object> {{"id", id}};
            return SelectOne<AnnouncementType>(conds);
        }
        public IList<AnnouncementType> GetList()
        {
            return SelectMany<AnnouncementType>();
        } 
    }
}
