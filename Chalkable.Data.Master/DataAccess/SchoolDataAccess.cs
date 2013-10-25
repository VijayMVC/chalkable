using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class SchoolDataAccess : DataAccessBase<School>
    {
        public SchoolDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public School GetSchoolById(Guid id)
        {
            var sql = @"select 
	                        School.Id as School_Id,
	                        School.DistrictRef as School_Id,
	                        School.LocalId as School_LocalId, 
	                        School.Name as School_Name, 
	                        District.Id as District_Id, 
	                        District.Name as District_Name, 
	                        District.SisUrl as District_SisUrl, 
	                        District.DbName as District_DbName, 
	                        District.SisUserName as District_SisUserName, 
	                        District.SisPassword as District_SisPassword, 
	                        District.[Status] as District_Status, 
	                        District.TimeZone as District_TimeZone, 
	                        District.DemoPrefix as District_DemoPrefix, 
	                        District.LastUseDemo as District_LastUseDemo, 
	                        District.ServerUrl as District_ServerUrl, 
	                        District.IsEmpty as District_IsEmpty
                        from 
                        school
                        join District on School.DistrictRef = District.Id
                        where
	                        School.Id = @{1}
                        ";
            sql = string.Format(sql, School.ID_FIELD);
            using (var reader = ExecuteReaderParametrized(sql, new Dictionary<string, object> { { School.ID_FIELD, id } }))
            {
                var res = reader.Read<School>(true);
                res.District = reader.Read<District>(true);
                return res;
            }
        }

        public PaginatedList<School> GetSchools(Guid? districtId, int start, int count)
        {
            var conds = new AndQueryCondition();
            if (districtId.HasValue)
                conds.Add(School.DISTRICT_REF_FIELD, districtId);
            return PaginatedSelect<School>(conds, School.ID_FIELD, start, count);
        }
        
    }
}