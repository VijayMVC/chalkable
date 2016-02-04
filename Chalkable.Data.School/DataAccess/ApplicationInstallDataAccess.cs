using System;
using System.Collections.Generic;
using System.Data;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.Data.School.DataAccess
{
    public class ApplicationInstallDataAccess : DataAccessBase<ApplicationInstall, int>
    {
        public ApplicationInstallDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<ApplicationInstall> GetInstalled(int personId, int schoolYearId)
        {
            var conds = new AndQueryCondition
            {
                {ApplicationInstall.SCHOOL_YEAR_REF_FIELD, schoolYearId },
                {ApplicationInstall.ACTIVE_FIELD, true },
                new OrQueryCondition
                    {
                        {ApplicationInstall.OWNER_REF_FIELD, personId },
                        { ApplicationInstall.PERSON_REF_FIELD, personId}
                    }
            };
            return SelectMany<ApplicationInstall>(conds);
        }

        public IList<ApplicationInstall> GetInstalledForAdmin(int personId, int acadYear)
        {
            var conds = new AndQueryCondition
            {
                {ApplicationInstall.ACTIVE_FIELD, true},
                new OrQueryCondition
                {
                    {ApplicationInstall.OWNER_REF_FIELD, personId},
                    {ApplicationInstall.PERSON_REF_FIELD, personId}
                }
            };
            var query = Orm.SimpleSelect<ApplicationInstall>(conds);
            var syQuery = new DbQuery();
            syQuery.Sql.AppendFormat(Orm.SELECT_FORMAT, SchoolYear.ID_FIELD, nameof(SchoolYear));
            var syConds = new AndQueryCondition
            {
                {SchoolYear.ARCHIVE_DATE, null, ConditionRelation.Equal},
                {SchoolYear.ACAD_YEAR_FIELD,  acadYear}
            };
            syConds.BuildSqlWhere(syQuery, nameof(SchoolYear));

            query.Sql.AppendFormat(" and {0} in ({1})", ApplicationInstall.SCHOOL_YEAR_REF_FIELD, syQuery.Sql);
            query.AddParameters(syQuery.Parameters);

            return ReadMany<ApplicationInstall>(query);
        } 

        public IDictionary<Guid, int> GetNotInstalledStudentsCountPerApplication(int staffId, int classId, int markingPeriodId)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
                {
                    {"@markingPeriodId", markingPeriodId},
                    {"@classId", classId},
                    {"@staffId", staffId},
                };
            using (var reader = ExecuteStoredProcedureReader("spGetNotInstalledStudentsCountPerApplication", ps))
            {
                var res = new Dictionary<Guid, int>();
                while (reader.Read())
                {
                    var appId = SqlTools.ReadGuid(reader, "ApplicationId");
                    var notInstalledStCount = SqlTools.ReadInt32(reader, "NotInstalledStudentCount");
                    res.Add(appId, notInstalledStCount);
                }
                return res;
            }
        } 

        public IList<ApplicationInstall> GetInstalledByAppId(Guid applicationId, int schoolYearId)
        {
            return SelectMany<ApplicationInstall>(new AndQueryCondition
                {
                    {ApplicationInstall.APPLICATION_REF_FIELD, applicationId},
                    {ApplicationInstall.SCHOOL_YEAR_REF_FIELD, schoolYearId},
                    {ApplicationInstall.ACTIVE_FIELD, true},
                });
        }

        public IList<PersonsForApplicationInstall> GetPersonsForApplicationInstall(Guid applicationId, int callerId, int? personId, IList<int> classes
            , int callerRoleId, bool hasAdminMyApps, bool hasTeacherMyApps, bool hasStudentMyApps, bool hasAdminExternalAttach
            , bool hasStudentExternalAttach, bool hasTeacherExternalAttach, bool canAttach, int schoolYearId)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
                {
                    {"applicationId", applicationId},
                    {"callerId", callerId},
                    {"personId", personId},
                    {"classIds", classes ?? new List<int>()},
                    {"callerRoleId", callerRoleId},
                    {"hasAdminMyApps", hasAdminMyApps},
                    {"hasTeacherMyApps", hasTeacherMyApps},
                    {"hasStudentMyApps", hasStudentMyApps},
                    {"hasStudentExternalAttach", hasStudentExternalAttach},
                    {"hasTeacherExternalAttach", hasTeacherExternalAttach},
                    {"hasAdminExternalAttach", hasAdminExternalAttach},
                    {"canAttach", canAttach},
                    {"schoolYearId", schoolYearId}
                };
            using (var reader = ExecuteStoredProcedureReader("spGetPersonsForApplicationInstall", ps))
            {
                return reader.ReadList<PersonsForApplicationInstall>();
            }
        }

        public IList<PersonsForApplicationInstallCount> GetPersonsForApplicationInstallCount(Guid applicationId, int callerId, int? personId, IList<int> classes
            , int callerRoleId, bool hasAdminMyApps, bool hasTeacherMyApps, bool hasStudentMyApps, bool hasAdminExternalAttach
            , bool hasStudentExternalAttach, bool hasTeacherExternalAttach, bool canAttach, int schoolYearId)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
                {
                    {"applicationId", applicationId},
                    {"callerId", callerId},
                    {"personId", personId},
                    {"callerRoleId", callerRoleId},
                    {"hasAdminMyApps", hasAdminMyApps},
                    {"hasTeacherMyApps", hasTeacherMyApps},
                    {"hasStudentMyApps", hasStudentMyApps},
                    {"hasStudentExternalAttach", hasStudentExternalAttach},
                    {"hasTeacherExternalAttach", hasTeacherExternalAttach},
                    {"hasAdminExternalAttach", hasAdminExternalAttach},
                    {"canAttach", canAttach},
                    {"schoolYearId", schoolYearId},
                    {"classIds", classes ?? new List<int>()  }
                };
            using (var reader = ExecuteStoredProcedureReader("spGetPersonsForApplicationInstallCount", ps))
            {
                return reader.ReadList<PersonsForApplicationInstallCount>();
            }
        }

        public IList<StudentCountToAppInstallByClass> GetStudentCountToAppInstallByClass(Guid applicationId, int schoolYearId, int personId, int roleId)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
                {
                    {"applicationId", applicationId},
                    {"schoolYearId", schoolYearId},
                    {"personId", personId},
                    {"roleId", roleId}
                };
            using (var reader = ExecuteStoredProcedureReader("spGetStudentCountToAppInstallByClass", ps))
            {
                return reader.ReadList<StudentCountToAppInstallByClass>();
            }
        }
    }

    public class ApplicationInstallActionClassesDataAccess : DataAccessBase<ApplicationInstallActionClasses, int>
    {
        public ApplicationInstallActionClassesDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }

}