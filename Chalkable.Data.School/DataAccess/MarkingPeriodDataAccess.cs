using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class MarkingPeriodDataAccess : DataAccessBase
    {
        public MarkingPeriodDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public void Create(MarkingPeriod markingPeriod)
        {
            SimpleInsert(markingPeriod);
        }
        public void Update(MarkingPeriod markingPeriod)
        {
            SimpleUpdate(markingPeriod);
        }
        public void Delete(MarkingPeriod markingPeriod)
        {
            SimpleDelete(markingPeriod);
        }

        public MarkingPeriod GetById(Guid id)
        {
            var conds = new Dictionary<string, object> {{"id", id}};
            return SelectOne<MarkingPeriod>(conds);
        }

    }
}
