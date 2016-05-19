using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class ApplicationSchoolOptionDataAccess : DataAccessBase<ApplicationSchoolOption>
    {
        public ApplicationSchoolOptionDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void BanSchoolsByIds(Guid applicationId, IList<Guid> schoolIds)
        {
            var @params = new Dictionary<string, object>
            {
                ["@applicationId"] = applicationId,
                ["@schoolIds"] = schoolIds ?? new List<Guid>()
            };
            ExecuteStoredProcedure("spBanSchoolsByIds", @params);
        }

        public IList<ApplicationBanInfo> GetApplicationBanInfos(Guid districtId, Guid? schoolId, IList<Guid> applicationIds)
        {
            var @params = new Dictionary<string, object>
            {
                ["applicationIds"] = applicationIds,
                ["districtId"] = districtId,
                ["schoolId"] = schoolId
            };

            return ExecuteStoredProcedureList<ApplicationBanInfo>("spGetApplicationsBanInfo", @params);
        }

        public IList<ApplicationSchoolBan> GetApplicationSchoolBans(Guid districtId, Guid applicationId)
        {
            var sql = $@"Select 
	                        @applicationId    As {nameof(ApplicationSchoolBan.ApplicationRef)}, 
	                        School.Id         As {nameof(ApplicationSchoolBan.SchoolRef)},
                            School.Name       As {nameof(ApplicationSchoolBan.SchoolName)},
	                        IsNull(Banned, 0) As {nameof(ApplicationSchoolBan.Banned)}
                        From 
	                        {nameof(School)} left join {nameof(ApplicationSchoolOption)}
		                        On {nameof(School.Id)} = {nameof(ApplicationSchoolOption.SchoolRef)}
                                    And {nameof(ApplicationSchoolOption.ApplicationRef)} = @applicationId
                        Where 
                            {nameof(School.DistrictRef)} = @districtId";

            var @params = new Dictionary<string, object>
            {
                ["districtId"] = districtId,
                ["applicationId"] = applicationId
            };

            using (var reader = ExecuteReaderParametrized(sql, @params))
            {
                return reader.ReadList<ApplicationSchoolBan>();
            }
        }
    }
}