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

        public IList<ApplicationInstall> GetInstalled(Guid personId)
        {
            var sql = string.Format("select * from ApplicationInstall where ({0}=@{0} or {1} = @{1}) and {2}=1",
                                    ApplicationInstall.OWNER_REF_FIELD, ApplicationInstall.PERSON_REF_FIELD, ApplicationInstall.ACTIVE_FIELD);
            var ps = new Dictionary<string, object>
                {
                    {ApplicationInstall.PERSON_REF_FIELD, personId},
                    {ApplicationInstall.OWNER_REF_FIELD, personId}
                };
            return ReadMany<ApplicationInstall>(new DbQuery(sql, ps));
        }

        public IList<ApplicationInstall> GetInstalledForClass(Class clazz)
        {
            var sql = string.Format(@"select ApplicationInstall.* from 
	                                        ApplicationInstall
	                                        join ApplicationInstallAction on ApplicationInstall.AppInstallActionRef = ApplicationInstallAction.Id
                                        where 
	                                        exists (select * from ApplicationInstallActionClasses where AppInstallActionRef = ApplicationInstallAction.Id and {0} = @{0})
	                                        and exists (select * from ApplicationInstallActionGradeLevel where AppInstallActionRef = ApplicationInstallAction.Id and {1} = @{1})
	                                        and {2} = 1", ApplicationInstallActionClasses.CLASS_REF_FIELD, ApplicationInstallActionGradeLevel.GRADE_LEVEL_REF_FIELD, ApplicationInstall.ACTIVE_FIELD);
            var ps = new Dictionary<string, object>
                {
                    {ApplicationInstallActionClasses.CLASS_REF_FIELD, clazz.Id},
                    {ApplicationInstallActionGradeLevel.GRADE_LEVEL_REF_FIELD, clazz.GradeLevelRef}
                };
            return ReadMany<ApplicationInstall>(new DbQuery(sql, ps));
        }

        public IList<ApplicationInstall> GetInstalledByAppId(Guid applicationId, Guid schoolYearId)
        {
            return SelectMany<ApplicationInstall>(new AndQueryCondition
                {
                    {ApplicationInstall.APPLICATION_REF_FIELD, applicationId},
                    {ApplicationInstall.SCHOOL_YEAR_REF_FIELD, schoolYearId},
                    {ApplicationInstall.ACTIVE_FIELD, true},
                });
        }

        public IList<PersonsForApplicationInstall> GetPersonsForApplicationInstall(Guid applicationId, Guid callerId, Guid? personId, IList<int> roles, IList<Guid> departments
            ,IList<Guid> gradeLevels, IList<Guid> classes, int callerRoleId, bool hasAdminMyApps, bool hasTeacherMyApps, bool hasStudentMyApps, bool canAttach)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
                {
                    {"applicationId", applicationId},
                    {"callerId", callerId},
                    {"personId", personId},
                    {"roleIds", roles.JoinString(",")},
                    {"departmentIds", departments.JoinString(",")},
                    {"gradeLevelIds", gradeLevels.JoinString(",")},
                    {"classIds", classes.JoinString(",")},
                    {"callerRoleId", callerRoleId},

                    {"hasAdminMyApps", hasAdminMyApps},
                    {"hasTeacherMyApps", hasTeacherMyApps},
                    {"hasStudentMyApps", hasStudentMyApps},
                    {"canAttach", canAttach},
                };
            using (var reader = ExecuteStoredProcedureReader("spGetPersonsForApplicationInstall", ps))
            {
                return reader.ReadList<PersonsForApplicationInstall>();
            }
        }

        public IList<PersonsForApplicationInstallCount> GetPersonsForApplicationInstallCount(Guid applicationId, Guid callerId, Guid? personId, IList<int> roles, IList<Guid> departments
            , IList<Guid> gradeLevels, IList<Guid> classes, int callerRoleId, bool hasAdminMyApps, bool hasTeacherMyApps, bool hasStudentMyApps, bool canAttach)
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
                };
            ps.Add("roleIds", roles == null || roles.Count == 0 ? null : roles.JoinString(","));
            ps.Add("departmentIds", departments == null || departments.Count == 0 ? null : departments.JoinString(","));
            ps.Add("gradeLevelIds", gradeLevels == null || gradeLevels.Count == 0 ? null : gradeLevels.JoinString(","));
            ps.Add("classIds", classes == null || classes.Count == 0 ? null : classes.JoinString(","));
           
            using (var reader = ExecuteStoredProcedureReader("spGetPersonsForApplicationInstallCount", ps))
            {
                return reader.ReadList<PersonsForApplicationInstallCount>();
            }
        }

        public IList<StudentCountToAppInstallByClass> GetStudentCountToAppInstallByClass(Guid applicationId, Guid schoolYearId, Guid personId, int roleId)
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

    public class ApplicationInstallActionDataAccess : DataAccessBase<ApplicationInstallAction, int>
    {
        public ApplicationInstallActionDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
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