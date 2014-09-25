using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoApplicationInstallStorage:BaseDemoIntStorage<ApplicationInstall>
    {
        public DemoApplicationInstallStorage(DemoStorage storage)
            : base(storage, x => x.Id, true)
        {
        }

        public IList<ApplicationInstall> GetAll(int personId)
        {
            return data.Where(x => x.Value.PersonRef== personId).Select(x => x.Value).ToList();
        }

        public IList<ApplicationInstall> GetInstalledForClass(ClassDetails clazz)
        {
            var persons = Storage.PersonStorage.GetPersons(new PersonQuery
            {
                ClassId = clazz.Id
            }).Persons.Select(x => x.Id);

            return data.Where(x => persons.Contains(x.Value.PersonRef)).Select(x => x.Value).ToList();

        }

        public IList<ApplicationInstall> GetInstalledByAppId(Guid applicationId, int schoolYearId)
        {
            return
                data.Where(x => x.Value.ApplicationRef == applicationId && x.Value.SchoolYearRef == schoolYearId)
                    .Select(x => x.Value)
                    .ToList();
        }

        public IList<PersonsForApplicationInstall> GetPersonsForApplicationInstall(Guid applicationId, int value, int? personId, IList<int> roleIds,
            IList<Guid> departmentIds, IList<int> gradeLevelIds, IList<int> classIds, int id, 
            bool hasAdminMyApps, bool hasTeacherMyApps, bool hasStudentMyApps, bool canAttach, int schoolYearId)
        {

            var callerRoleId = Storage.Context.RoleId;
            var callerId = Storage.Context.PersonId;

            var canInstallForTeacher = hasTeacherMyApps || canAttach;
            var canInstallForStudent = hasStudentMyApps || canAttach;

            var canInstall = CanInstall(hasAdminMyApps, hasStudentMyApps, callerRoleId, canInstallForStudent, canInstallForTeacher);

            var schoolId = Storage.SchoolYearStorage.GetById(schoolYearId).SchoolRef;

            var personsForInstall = new List<KeyValuePair<int, int>>();

            if (canInstall)
            {
                if (callerRoleId == CoreRoles.STUDENT_ROLE.Id)
                {
                    var sp =
                        Storage.SchoolPersonStorage.GetAll()
                            .First(x => x.PersonRef == callerId && x.SchoolRef == schoolId && hasStudentMyApps);
                    personsForInstall.Add(new KeyValuePair<int, int>(sp.PersonRef, sp.RoleRef));
                }

                if (callerRoleId == CoreRoles.TEACHER_ROLE.Id)
                {
                    var classes = Storage.ClassStorage.GetClassesComplex(new ClassQuery
                    {
                        PersonId = callerId
                    }).Classes.Select(x => x.Id);

                    var personRefs =
                        Storage.ClassPersonStorage.GetAll()
                            .Where(x => classes.Contains(x.ClassRef))
                            .Select(x => x.PersonRef);

                    var sps =
                        Storage.SchoolPersonStorage.GetAll()
                            .Where(x => (personRefs.Contains(x.PersonRef) && canInstallForStudent || x.PersonRef == callerId && canInstallForTeacher)
                                && x.SchoolRef == schoolId);
                    
                    personsForInstall.AddRange(sps.Select(schoolPerson => new KeyValuePair<int, int>(schoolPerson.PersonRef, schoolPerson.RoleRef)));
                }
            }

            var installed =
                Storage.ApplicationInstallStorage.GetAll()
                    .Where(x => x.Active && x.ApplicationRef == applicationId).Select(x => x.PersonRef)
                    .ToList();

            personsForInstall = personsForInstall.Where(x => !installed.Contains(x.Key)).ToList();

            var result = new List<PersonsForApplicationInstall>();
            PrepareRoleInstalls(roleIds, result, personsForInstall);
            PrepareDepartmentInstalls(departmentIds, personsForInstall, result);
            PrepareClassInstalls(classIds, personsForInstall, result, callerRoleId);
            PreparePersonInstalls(personId, roleIds, departmentIds, gradeLevelIds, classIds, callerRoleId, callerId, result, personsForInstall);
            PrepareGradeLevelInstalls(gradeLevelIds, schoolYearId, personsForInstall, result);
            return result;
        }

        private static bool CanInstall(bool hasAdminMyApps, bool hasStudentMyApps, int callerRoleId, bool canInstallForStudent,
            bool canInstallForTeacher)
        {
            var canInstall = callerRoleId == CoreRoles.STUDENT_ROLE.Id && hasStudentMyApps ||
                             (callerRoleId == CoreRoles.TEACHER_ROLE.Id &&
                              (canInstallForStudent || canInstallForTeacher))
                             ||
                             ((callerRoleId == CoreRoles.ADMIN_EDIT_ROLE.Id ||
                               callerRoleId == CoreRoles.ADMIN_GRADE_ROLE.Id ||
                               callerRoleId == CoreRoles.ADMIN_VIEW_ROLE.Id)
                              && (hasAdminMyApps || canInstallForStudent || canInstallForTeacher));
            return canInstall;
        }

        private static void PrepareRoleInstalls(ICollection<int> roleIds, List<PersonsForApplicationInstall> result, IEnumerable<KeyValuePair<int, int>> personsForInstall)
        {
            if (roleIds == null) return;
            result.AddRange(
                personsForInstall.Where(x => roleIds.Contains(x.Value)).Select(x => new PersonsForApplicationInstall
                {
                    GroupId = x.Key.ToString(CultureInfo.InvariantCulture),
                    PersonId = x.Key,
                    Type = PersonsFroAppInstallTypeEnum.Role
                }));
        }

        private void PrepareDepartmentInstalls(ICollection<Guid> departmentIds, IEnumerable<KeyValuePair<int, int>> personsForInstall, List<PersonsForApplicationInstall> result)
        {
            if (departmentIds == null) return;

            var personRefs = personsForInstall.Select(x => x.Key).ToList();

            var filtered =
                Storage.ClassPersonStorage.GetAll().Where(x =>
                {
                    var cls = Storage.ClassStorage.GetById(x.ClassRef);
                    return personRefs.Contains(x.PersonRef) && cls.ChalkableDepartmentRef != null &&
                           departmentIds.Contains(cls.ChalkableDepartmentRef.Value);
                }).Select(x =>
                {
                    var chalkableDepartmentRef = Storage.ClassStorage.GetById(x.ClassRef).ChalkableDepartmentRef;
                    return chalkableDepartmentRef != null
                        ? new
                        {
                            x.PersonRef,
                            DepartmentName = Storage.ChalkableDepartmentStorage.GetById(chalkableDepartmentRef.Value).Name
                        }
                        : new {x.PersonRef, DepartmentName = ""};
                });

            result.AddRange(filtered.Select(x => new PersonsForApplicationInstall
            {
                GroupId = x.DepartmentName,
                PersonId = x.PersonRef,
                Type = PersonsFroAppInstallTypeEnum.Department
            }));
        }

        private void PrepareClassInstalls(IList<int> classIds, IEnumerable<KeyValuePair<int, int>> personsForInstall, 
            List<PersonsForApplicationInstall> result, int callerRoleId)
        {
            if (classIds == null) return;
            var ids = personsForInstall.Select(x => x.Key).ToList();
            foreach (var classId in classIds)
            {
                result.AddRange(
                    Storage.ClassPersonStorage.GetAll()
                        .Where(x => x.ClassRef == classId && ids.Contains(x.PersonRef))
                        .Select(x => new PersonsForApplicationInstall()
                        {
                            GroupId = classId.ToString(CultureInfo.InvariantCulture),
                            PersonId = x.PersonRef,
                            Type = PersonsFroAppInstallTypeEnum.Class
                        }));
            }

            if (callerRoleId == CoreRoles.TEACHER_ROLE.Id) return;
            foreach (var classId in classIds)
            {
                result.AddRange(
                    Storage.ClassStorage.GetAll()
                        .Where(cls => cls.Id == classId && cls.PrimaryTeacherRef != null && ids.Contains(cls.PrimaryTeacherRef.Value))
                        .Select(x => new PersonsForApplicationInstall()
                        {
                            GroupId = classId.ToString(CultureInfo.InvariantCulture),
                            PersonId = x.PrimaryTeacherRef.Value,
                            Type = PersonsFroAppInstallTypeEnum.Class
                        }));
            }
        }

        private static void PreparePersonInstalls(int? personId, IList<int> roleIds, IList<Guid> departmentIds, IList<int> gradeLevelIds,
            IList<int> classIds, int callerRoleId, int? callerId, List<PersonsForApplicationInstall> result, 
            List<KeyValuePair<int, int>> personsForInstall)
        {
            var isSinglePerson = false;

            if (callerRoleId == CoreRoles.TEACHER_ROLE.Id && !personId.HasValue)
            {
                personId = callerId;
                isSinglePerson = true;
            }

            if (personId.HasValue)
            {
                result.AddRange(personsForInstall.Where(x => x.Key == personId).Select(x => new PersonsForApplicationInstall
                {
                    GroupId = x.Key.ToString(CultureInfo.InvariantCulture),
                    PersonId = x.Key,
                    Type = PersonsFroAppInstallTypeEnum.Person
                }));
            }

            if (roleIds == null && departmentIds == null && gradeLevelIds == null && classIds == null &&
                (isSinglePerson || !personId.HasValue))
            {
                result.AddRange(personsForInstall.Select(x => new PersonsForApplicationInstall
                {
                    GroupId = x.Key.ToString(CultureInfo.InvariantCulture),
                    PersonId = x.Key,
                    Type = PersonsFroAppInstallTypeEnum.Person
                }));
            }
        }

        private void PrepareGradeLevelInstalls(ICollection<int> gradeLevelIds, int schoolYearId, List<KeyValuePair<int, int>> personsForInstall,
            List<PersonsForApplicationInstall> result)
        {
            if (gradeLevelIds == null) return;
            var personIds = personsForInstall.Select(x => x.Key).ToList();
            var ssyPersons =
                Storage.StudentSchoolYearStorage.GetAll()
                    .Where(
                        x =>
                            personIds.Contains(x.StudentRef) && x.SchoolYearRef == schoolYearId &&
                            gradeLevelIds.Contains(x.GradeLevelRef));

            result.AddRange(ssyPersons.Select(x => new PersonsForApplicationInstall
            {
                Type = PersonsFroAppInstallTypeEnum.GradeLevel,
                GroupId = x.GradeLevel.Number.ToString(CultureInfo.InvariantCulture),
                PersonId = x.StudentRef
            }));


            var teacherRefs = personsForInstall.Select(x => x.Key).ToList();
            var classes =
                Storage.ClassStorage.GetAll()
                    .Where(x => x.PrimaryTeacherRef != null && (gradeLevelIds.Contains(x.GradeLevelRef) && teacherRefs.Contains(x.PrimaryTeacherRef.Value)));

            result.AddRange(classes.Select(x => new PersonsForApplicationInstall
            {
                Type = PersonsFroAppInstallTypeEnum.GradeLevel,
                GroupId = Storage.GradeLevelStorage.GetById(x.GradeLevelRef).Number.ToString(CultureInfo.InvariantCulture),
                PersonId = x.PrimaryTeacherRef.Value
            }));
        }

        public IEnumerable<StudentCountToAppInstallByClass> GetStudentCountToAppInstallByClass(Guid applicationId, int schoolYearId, int userId, int roleId)
        {
            var classes = Storage.ClassStorage.GetClassesComplex(new ClassQuery
            {
                CallerRoleId = roleId,
                CallerId = userId,
                SchoolYearId = schoolYearId
            });
            var csps = Storage.ClassPersonStorage.GetAll().Where(cp=>classes.Classes.Any(c=>c.Id == cp.ClassRef)).ToList();
            var appInstalls = Storage.ApplicationInstallStorage.GetAll()
                .Where(x => x.ApplicationRef == applicationId && x.Active && x.SchoolYearRef == schoolYearId).ToList();

            return classes.Classes.Select(c => new StudentCountToAppInstallByClass
                {
                    ClassId = c.Id,
                    ClassName = c.Name,
                    NotInstalledStudentCount = csps.Count(cp=>cp.ClassRef == c.Id && appInstalls.All(install=>install.PersonRef != cp.PersonRef))
                }).ToList();


            //var classPersons = from cls in classes
            //    join csp in csps on cls.Id equals csp.ClassRef
            //    join appInst in appInstalls on new {csp.PersonRef, cls.SchoolYearRef} equals
            //        new {appInst.PersonRef, SchoolYearRef = (int?) appInst.SchoolYearRef} into cspJoined
            //    from cspj in cspJoined.DefaultIfEmpty()
            //    group cspj by new {cls.Id, cls.Name}
            //    into grouped
            //    select
            //        new
            //        {
            //            ClassId = grouped.Key.Id,
            //            ClassName = grouped.Key.Name,
            //            NotInstalledStudentCount = grouped.Count()
            //        };


            //return classPersons.Select(x => new StudentCountToAppInstallByClass
            //{
            //    ClassId = x.ClassId,
            //    ClassName = x.ClassName,
            //    NotInstalledStudentCount = x.NotInstalledStudentCount
            //});
        }

        public IList<PersonsForApplicationInstallCount> GetPersonsForApplicationInstallCount(Guid applicationId, int userId, int? personId, IList<int> roleIds, IList<Guid> departmentIds, IList<int> gradeLevelIds, IList<int> classIds, int id, bool hasAdminMyApps, bool hasTeacherMyApps, bool hasStudentMyApps, bool canAttach, int schoolYearId)
        {
            var personsForAppInstall = GetPersonsForApplicationInstall(applicationId, userId, personId, roleIds, departmentIds,
                gradeLevelIds, classIds, id, hasAdminMyApps, hasTeacherMyApps, hasStudentMyApps, canAttach, schoolYearId);

            var res =
                (from p in personsForAppInstall
                group p by new {p.Type, p.GroupId}
                into gr
                select new
                {
                    gr.ToList().Count, 
                    gr.Key.GroupId, 
                    gr.Key.Type
                }).Union(new[] {new
                {
                    personsForAppInstall.Select(x => x.PersonId).Distinct().ToList().Count,
                    GroupId = "",
                    Type = PersonsFroAppInstallTypeEnum.Total
                }});

            return res.Select(x => new PersonsForApplicationInstallCount
            {
                Type = x.Type,
                Count = x.Count,
                GroupId = x.GroupId
            }).ToList();
        }

        public bool Exists(Guid applicationRef, int personId)
        {
            return data.Count(x => x.Value.ApplicationRef == applicationRef && x.Value.PersonRef == personId) > 0;
        }

        public IList<ApplicationInstall> GetInstalled(int personId)
        {
            return
                data.Where(x => (x.Value.OwnerRef == personId || x.Value.PersonRef == personId) && x.Value.Active)
                    .Select(x => x.Value)
                    .ToList();
        }
    
        public IList<ApplicationInstall> GetAll(Guid applicationId, bool personId)
        {
            return
                data.Where(
                    x => x.Value.ApplicationRef == applicationId && x.Value.Active)
                    .Select(x => x.Value)
                    .ToList();
        }

        public IList<ApplicationInstall> GetAll(Guid applicationId, int personId, bool active, bool ownersOnly = false)
        {
            var apps = data.Where(x => x.Value.ApplicationRef == applicationId && x.Value.Active)
                .Select(x => x.Value);
            apps = ownersOnly ? apps.Where(x => x.OwnerRef == personId) : apps.Where(x => x.PersonRef == personId);
            return apps.ToList();

        }
    }
}
