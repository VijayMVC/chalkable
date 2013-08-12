using System;
using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class FundDataAccess : DataAccessBase<Fund>
    {
        public FundDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<Fund> GetRoleFunds(Guid schoolId, int[] roleIds)
        {
            var sql = string.Format(@"select Fund.* from Fund 
                    left join [User] u1 on Fund.FromUserRef = u1.Id 
                    left join [User] u2 on Fund.ToUserRef = u2.Id
                    left join [SchoolUser] su1 on u1.Id = su1.UserRef
                    left join [SchoolUser] su2 on u2.Id = su2.UserRef
                    where 
                        Fund.{0} = @{0} and (su1.{1} in ({2}) or su2.{1} in ({2}))"
                , Fund.SCHOOL_REF_FIELD, SchoolUser.ROLE_FIELD, roleIds.JoinString(","));
            var ps = new Dictionary<string, object> { {Fund.SCHOOL_REF_FIELD, schoolId} };
            return ReadMany<Fund>(new DbQuery {Parameters = ps, Sql = sql});

        }

        public decimal GetPaymentsForApps(Guid schoolId)
        {
            var sql = string.Format("select sum(amount) as [Amount] from Fund where [{0}] is not null and [{1}] is not null and [{2}]=@{2}"
                    , Fund.APP_INSTALL_ACTION_REF_FIELD, Fund.FROM_USER_REF_FIELD, Fund.SCHOOL_REF_FIELD);
            IDictionary<string, object> ps = new Dictionary<string, object> { { Fund.SCHOOL_REF_FIELD , schoolId} };
            using (var reader = ExecuteReaderParametrized(sql, ps))
            {
                reader.Read();
                return SqlTools.ReadDecimal(reader, Fund.AMOUNT_FIELD);
            }
        }
    }

    public class FundRequestDataAccess : DataAccessBase<FundRequest>
    {
        public FundRequestDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }

    public class FundRequestRoleDistributionDataAccess : DataAccessBase<FundRequestRoleDistribution>
    {
        public FundRequestRoleDistributionDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}