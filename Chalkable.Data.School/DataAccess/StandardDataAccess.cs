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
    public class StandardDataAccess : DataAccessBase<Standard, int>
    {
        public StandardDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override void Delete(int key)
        {
            var q = new DbQuery(new List<DbQuery>
                {
                    Orm.SimpleDelete<AnnouncementStandard>(new AndQueryCondition { {AnnouncementStandard.STANDARD_REF_FIELD, key} }),
                    Orm.SimpleDelete<ClassStandard>(new AndQueryCondition {{ClassStandard.STANDARD_REF_FIELD, key}}),
                    Orm.SimpleDelete<Standard>(new AndQueryCondition {{Standard.ID_FIELD, key}})
                });
            ExecuteNonQueryParametrized(q.Sql.ToString(), q.Parameters);
        }
    }

    public class StandardSubjectDataAccess : DataAccessBase<StandardSubject, int>
    {
        public StandardSubjectDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }

    public class ClassStandardDataAccess: DataAccessBase<ClassStandard, int>
    {
        public ClassStandardDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }

    public class AnnouncementStandardDataAccess : DataAccessBase<AnnouncementStandard, int>
    {
        public AnnouncementStandardDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
