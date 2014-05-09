using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading;
using Chalkable.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoApplicationInstallStorage:BaseDemoStorage<int, ApplicationInstall>
    {
        public DemoApplicationInstallStorage(DemoStorage storage)
            : base(storage)
        {
        }

        public void Add(List<ApplicationInstall> appInstall)
        {
            foreach (var applicationInstall in appInstall)
            {
                Add(applicationInstall);
            }
        }

        public void Add(ApplicationInstall appInstall)
        {
            appInstall.Id = GetNextFreeId();
            appInstall.Active = true;
            data[appInstall.Id] = appInstall;
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
            var callerId = Storage.Context.UserLocalId;

            var canInstallForTeacher = hasTeacherMyApps || canAttach;
            var canInstallForStudent = hasStudentMyApps || canAttach;

            var canInstall = callerRoleId == CoreRoles.STUDENT_ROLE.Id || hasStudentMyApps ||
                             (callerRoleId == CoreRoles.TEACHER_ROLE.Id &&
                              (canInstallForStudent || canInstallForTeacher))
                             ||
                             ((callerRoleId == CoreRoles.ADMIN_EDIT_ROLE.Id ||
                               callerRoleId == CoreRoles.ADMIN_GRADE_ROLE.Id ||
                               callerRoleId == CoreRoles.ADMIN_VIEW_ROLE.Id)
                              && (hasAdminMyApps || canInstallForStudent || canInstallForTeacher));

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
                            .Where(x => (personRefs.Contains(x.PersonRef) && canInstallForStudent || x.PersonRef == callerId && canInstallForTeacher) && x.SchoolRef == schoolId);

                    personsForInstall.AddRange(sps.Select(schoolPerson => new KeyValuePair<int, int>(schoolPerson.PersonRef, schoolPerson.RoleRef)));
                }

                if (callerRoleId == CoreRoles.ADMIN_VIEW_ROLE.Id || callerRoleId == CoreRoles.ADMIN_EDIT_ROLE.Id ||
                    callerRoleId == CoreRoles.ADMIN_GRADE_ROLE.Id)
                {
                    var sps = Storage.SchoolPersonStorage.GetAll().Where(x =>
                        (x.RoleRef == CoreRoles.TEACHER_ROLE.Id && canInstallForTeacher) ||
                        (x.RoleRef == CoreRoles.ADMIN_VIEW_ROLE.Id || x.RoleRef == CoreRoles.ADMIN_EDIT_ROLE.Id ||
                         x.RoleRef == CoreRoles.ADMIN_GRADE_ROLE.Id) && hasAdminMyApps
                        || (x.RoleRef == CoreRoles.STUDENT_ROLE.Id && canInstallForStudent) && x.SchoolRef == schoolId)
                        .ToList();
                    personsForInstall.AddRange(sps.Select(schoolPerson => new KeyValuePair<int, int>(schoolPerson.PersonRef, schoolPerson.RoleRef)));


                }

            }

            var installed =
                Storage.ApplicationInstallStorage.GetAll()
                    .Where(x => x.Active && x.ApplicationRef == applicationId).Select(x => x.PersonRef)
                    .ToList();

            personsForInstall = personsForInstall.Where(x => !installed.Contains(x.Key)).ToList();


            var result = new List<PersonsForApplicationInstall>();


            if (roleIds != null)
            {
                result.AddRange(personsForInstall.Where(x => roleIds.Contains(x.Value)).Select(x => new PersonsForApplicationInstall
                {
                    GroupId = x.Key.ToString(),
                    PersonId = x.Key,
                    Type = PersonsFroAppInstallTypeEnum.Role
                }));
    
            }

            if (departmentIds != null)
            {
                var personRefs = personsForInstall.Select(x => x.Key).ToList();
     
                var filtered =
                    Storage.ClassPersonStorage.GetAll().Where(x =>
                    {
                        var cls = Storage.ClassStorage.GetById(x.ClassRef);
                        return personRefs.Contains(x.PersonRef) && cls.ChalkableDepartmentRef != null && departmentIds.Contains(cls.ChalkableDepartmentRef.Value);
                    }).Select(x =>
                    {
                        var chalkableDepartmentRef = Storage.ClassStorage.GetById(x.ClassRef).ChalkableDepartmentRef;
                        return chalkableDepartmentRef != null ? new {PersonRef = x.PersonRef, DepartmentName = Storage.ChalkableDepartmentStorage.GetById(chalkableDepartmentRef.Value).Name} : new {PersonRef = x.PersonRef, DepartmentName = ""};
                    });

                result.AddRange(filtered.Select(x => new PersonsForApplicationInstall
                {
                    GroupId = x.DepartmentName,
                    PersonId = x.PersonRef,
                    Type = PersonsFroAppInstallTypeEnum.Department
                }));
            }


            if (classIds != null)
            {
                var ids = personsForInstall.Select(x => x.Key).ToList();
                foreach (var classId in classIds)
                {
                    result.AddRange(
                        Storage.ClassPersonStorage.GetAll()
                            .Where(x => x.ClassRef == classId && ids.Contains(x.PersonRef))
                            .Select(x => new PersonsForApplicationInstall()
                            {
                                GroupId = classId.ToString(),
                                PersonId = x.PersonRef,
                                Type = PersonsFroAppInstallTypeEnum.Class
                            }));
                }

                if (callerRoleId != CoreRoles.TEACHER_ROLE.Id)
                {
                    foreach (var classId in classIds)
                    {
                        result.AddRange(
                            Storage.ClassStorage.GetAll()
                                .Where(x => x.Id == classId && x.TeacherRef != null && ids.Contains(x.TeacherRef.Value))
                                .Select(x => new PersonsForApplicationInstall()
                                {
                                    GroupId = classId.ToString(),
                                    PersonId = x.TeacherRef.Value,
                                    Type = PersonsFroAppInstallTypeEnum.Class
                                }));
                    }
                }


            }

            if (personId.HasValue)
            {
                result.AddRange(personsForInstall.Where(x => x.Key == personId).Select(x => new PersonsForApplicationInstall
                {
                    GroupId = x.Key.ToString(),
                    PersonId = x.Key,
                    Type = PersonsFroAppInstallTypeEnum.Person
                }));
            }


            var isSinglePerson = false;

            if (callerRoleId == CoreRoles.TEACHER_ROLE.Id && !personId.HasValue)
            {
                personId = callerId;
                isSinglePerson = true;
            }

            if (roleIds == null && departmentIds == null && gradeLevelIds == null && classIds == null &&
                (isSinglePerson || !personId.HasValue))
            {
                result.AddRange(personsForInstall.Where(x => x.Key == personId).Select(x => new PersonsForApplicationInstall
                {
                    GroupId = x.Key.ToString(),
                    PersonId = x.Key,
                    Type = PersonsFroAppInstallTypeEnum.Person
                }));
            }


         

          


            if (gradeLevelIds != null)
            {
                var personIds = personsForInstall.Select(x => x.Key).ToList();
                var ssyPersons =
                    Storage.StudentSchoolYearStorage.GetAll()
                        .Where(x => personIds.Contains(x.StudentRef) && x.SchoolYearRef == schoolYearId && gradeLevelIds.Contains(x.GradeLevelRef));

                result.AddRange(ssyPersons.Select(x => new PersonsForApplicationInstall
                {
                    Type = PersonsFroAppInstallTypeEnum.GradeLevel,
                    GroupId = x.GradeLevel.Number.ToString(),
                    PersonId = x.StudentRef
                }));



                var teacherRefs = personsForInstall.Select(x => x.Key).ToList();
                var classes = Storage.ClassStorage.GetAll().Where(x => x.TeacherRef != null && (gradeLevelIds.Contains(x.GradeLevelRef) && teacherRefs.Contains(x.TeacherRef.Value)));

                result.AddRange(classes.Select( x => new PersonsForApplicationInstall
                {
                    Type = PersonsFroAppInstallTypeEnum.GradeLevel,
                    GroupId = Storage.GradeLevelStorage.GetById(x.GradeLevelRef).Number.ToString(),
                    PersonId = x.TeacherRef.Value
                }));
            }
            return result;
        }


        public void Update(ApplicationInstall appInst)
        {
            if (data.ContainsKey(appInst.Id))
                data[appInst.Id] = appInst;

        }

        public IEnumerable<StudentCountToAppInstallByClass> GetStudentCountToAppInstallByClass(Guid applicationId, int schoolYearId, int userId, int roleId)
        {


            var appInstalls =
                Storage.ApplicationInstallStorage.GetAll().Where(x => x.ApplicationRef == applicationId && x.Active).ToList();

            var csps = Storage.ClassPersonStorage.GetAll().ToList();
            var classes = Storage.ClassStorage.GetAll().ToList();
            var classPersons = from cls in classes
                join csp in csps on cls.Id equals csp.ClassRef
                join appInst in appInstalls on new {csp.PersonRef, cls.SchoolYearRef} equals
                    new {appInst.PersonRef, SchoolYearRef = (int?) appInst.SchoolYearRef} into cspJoined
                from cspj in cspJoined.DefaultIfEmpty()
                group cspj by new {cls.Id, cls.Name}
                into grouped
                select
                    new
                    {
                        ClassId = grouped.Key.Id,
                        ClassName = grouped.Key.Name,
                        NotInstalledStudentCount = grouped.Count()
                    };


            return classPersons.Select(x => new StudentCountToAppInstallByClass
            {
                ClassId = x.ClassId,
                ClassName = x.ClassName,
                NotInstalledStudentCount = x.NotInstalledStudentCount
            });
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
                    Count = gr.ToList().Count,
                    GroupId = gr.Key.GroupId,
                    Type = gr.Key.Type
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

        public override void Setup()
        {
            throw new NotImplementedException();
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
