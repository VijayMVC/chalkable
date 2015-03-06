using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AlternateScoreDataAccess : DataAccessBase<AlternateScore,int>
    {
        public AlternateScoreDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Delete(IList<int> ids)
        {
            SimpleDelete<AlternateScore>(ids.Select(x => new AlternateScore {Id = x}).ToList());
        }
    }
}
