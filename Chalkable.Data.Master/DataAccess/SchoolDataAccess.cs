using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class SchoolDataAccess : DataAccessBase<School, Guid>
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
            using (var reader = ExecuteReaderParametrized(sql, new Dictionary<string, object> {{School.ID_FIELD, id}}))
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

        public void Delete(IList<Guid> ids)
        {
            SimpleDelete(ids.Select(x => new School {Id = x}).ToList());
        }

        public void UpdateStudyCenterEnabled(Guid? districtId, Guid? schoolId, DateTime? enabledTill)
        {
            Trace.Assert(districtId.HasValue != schoolId.HasValue);
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@districtId", districtId},
                {"@schoolId", schoolId},
                {"@enabledTill", enabledTill}
            };
            ExecuteStoredProcedure("spUpdateStudyCenterEnabled", ps);
        }

        public void UpdateMessagingDisabled(Guid? districtId, Guid? schoolId, bool disabled)
        {
            Trace.Assert(districtId.HasValue != schoolId.HasValue);
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@districtId", districtId},
                {"@schoolId", schoolId},
                {"@disabled", disabled}
            };
            ExecuteStoredProcedure("spUpdateMessagingDisabled", ps);
        }

        public void UpdateMessagingSettings(Guid? districtId, Guid? schoolId, bool studentMessaging,
            bool studentToClassOnly, bool teacherToStudentMessaging, bool teacherToClassOnly)
        {
            Trace.Assert(districtId.HasValue != schoolId.HasValue);
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@districtId", districtId},
                {"@schoolId", schoolId},
                {"@studentMessaging", studentMessaging},
                {"@studentToClassOnly", studentToClassOnly},
                {"@teacherToStudentMessaging", teacherToStudentMessaging},
                {"@teacherToClassOnly", teacherToClassOnly}
            };
            ExecuteStoredProcedure("spUpdateMessagingSettings", ps);

        }

        public MessagingSettings GetDistrictMessagingSettings(Guid districtId)
        {
            var res = new DbQuery();
            var schoolN = nameof(School);
            res.Sql.Append(
                $@"Select
	                             cast(Min(cast(School.StudentMessagingEnabled as int)) as bit) as StudentMessagingEnabled,
	                             cast(Min(cast(School.StudentToClassMessagingOnly as int)) as bit) as StudentToClassMessagingOnly,
	                             cast(Min(cast(School.TeacherToStudentMessaginEnabled as int)) as bit) as TeacherToStudentMessaginEnabled,
	                             cast(Min(cast(School.TeacherToClassMessagingOnly as int)) as bit) as TeacherToClassMessagingOnly
                              From School
                          ");
            new AndQueryCondition {{School.DISTRICT_REF_FIELD, districtId}}.BuildSqlWhere(res, schoolN);
            res.Sql.Append($" Group by {schoolN}.{School.DISTRICT_REF_FIELD}");
            return ReadOne<MessagingSettings>(res);
        }

        public void UpdateAssessmentEnabled(Guid? districtId, Guid? schoolId, bool enabled)
        {
            Trace.Assert(districtId.HasValue != schoolId.HasValue);
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@districtId", districtId},
                {
                    "@schoolId", schoolId
                },
                {"@enabled", enabled}
            };
            ExecuteStoredProcedure("spUpdateAssessmentEnabled", ps);
        }
    }
}