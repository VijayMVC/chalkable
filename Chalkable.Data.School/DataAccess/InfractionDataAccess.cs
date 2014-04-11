using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class InfractionDataAccess : DataAccessBase<Infraction, int>
    {
        public InfractionDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Delete(IList<short> ids)
        {
            SimpleDelete<Infraction>(ids.Select(x=> new Infraction{Id = x}).ToList());
        }

     
    }
}
