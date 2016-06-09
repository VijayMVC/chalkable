using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class GradingScaleDataAccess : DataAccessBase<GradingScale, int>
    {
        public GradingScaleDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<GradingScaleRange> GetClassGradingScaleRanges(int classId)
        {
            var query = 
                $@"Select {nameof(GradingScaleRange)}.* 
                   From {nameof(Class)} join {nameof(GradingScaleRange)}
	                    on {nameof(Class)}.{Class.GRADING_SCALE_REF_FIELD} = {nameof(GradingScaleRange)}.{GradingScaleRange.GRADING_SCALE_REF_FIELD}
                   Where {nameof(Class)}.{Class.ID_FIELD} = @classId";

            var @params = new Dictionary<string, object>
            {
                ["classId"] = classId
            };

            return ReadMany<GradingScaleRange>(new DbQuery(query, @params));
        }
    }
}
