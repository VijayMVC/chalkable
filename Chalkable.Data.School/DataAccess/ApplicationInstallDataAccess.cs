using System;
using System.Collections.Generic;
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

        public IList<ApplicationInstall> GetByIds(IList<int> applicationInstallIds)
        {
            var dbQuery = new DbQuery();
            dbQuery.Sql.Append("select * from ApplicationInstall");
            if (applicationInstallIds != null && applicationInstallIds.Count > 0)
                dbQuery.Sql.AppendFormat(" where Id in ({0})", applicationInstallIds.JoinString(","));
            return ReadMany<ApplicationInstall>(dbQuery);
        } 

        public IList<ApplicationInstall> GetInstalled(int personId, int schoolYearId)
        {
            var sql = string.Format("select * from ApplicationInstall where ({0}=@{0} or {1} = @{1}) and {2}=1 and {3}=@{3}"
                                    , ApplicationInstall.OWNER_REF_FIELD, ApplicationInstall.PERSON_REF_FIELD
                                    , ApplicationInstall.ACTIVE_FIELD, ApplicationInstall.SCHOOL_YEAR_REF_FIELD);
            var ps = new Dictionary<string, object>
                {
                    {ApplicationInstall.PERSON_REF_FIELD, personId},
                    {ApplicationInstall.OWNER_REF_FIELD, personId},
                    {ApplicationInstall.SCHOOL_YEAR_REF_FIELD, schoolYearId}
                };
            return ReadMany<ApplicationInstall>(new DbQuery(sql, ps));
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

        public IList<PersonsForApplicationInstall> GetPersonsForApplicationInstall(Guid applicationId, int callerId, int? personId, IList<int> classes, int callerRoleId, bool hasAdminMyApps, bool hasTeacherMyApps, bool hasStudentMyApps, bool canAttach, int schoolYearId)
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
                    {"canAttach", canAttach},
                    {"schoolYearId", schoolYearId}
                };
            using (var reader = ExecuteStoredProcedureReader("spGetPersonsForApplicationInstall", ps))
            {
                return reader.ReadList<PersonsForApplicationInstall>();
            }
        }

        public IList<PersonsForApplicationInstallCount> GetPersonsForApplicationInstallCount(Guid applicationId, int callerId, int? personId, IList<int> classes, int callerRoleId, bool hasAdminMyApps, bool hasTeacherMyApps, bool hasStudentMyApps, bool canAttach, int schoolYearId)
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

    public class ApplicationInstallActionGradeLevelDataAccess : DataAccessBase<ApplicationInstallActionGradeLevel, int>
    {
        public ApplicationInstallActionGradeLevelDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }

    public class ApplicationInstallActionDepartmentDataAccess : DataAccessBase<ApplicationInstallActionDepartment, int>
    {
        public ApplicationInstallActionDepartmentDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }

    public class ApplicationInstallActionRoleDataAccess : DataAccessBase<ApplicationInstallActionRole, int>
    {
        public ApplicationInstallActionRoleDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}