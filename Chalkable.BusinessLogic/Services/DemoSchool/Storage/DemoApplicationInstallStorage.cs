using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Web.Hosting;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;
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
            if (!data.ContainsKey(appInstall.Id))
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
            /*CREATE PROCEDURE [dbo].[spGetPersonsForApplicationInstall]
	            @applicationId uniqueidentifier, @callerId int, @personId int, @roleIds nvarchar(2048), @departmentIds nvarchar(2048)
	            , @gradeLevelIds nvarchar(2048), @classIds nvarchar(2048), @callerRoleId int
	            , @hasAdminMyApps bit, @hasTeacherMyApps bit, @hasStudentMyApps bit, @canAttach bit, @schoolYearId int
            as
       
            if(@roleIds is not null)
            begin
	            insert into @roleIdsT(value)
	            select cast(s as int) from dbo.split(',', @roleIds) 
            end
            if(@departmentIds is not null)
            begin
	            insert into @departmentIdsT(value)
	            select cast(s as uniqueidentifier) from dbo.split(',', @departmentIds)
            end
            if(@gradeLevelIds is not null)
            begin
	            insert into @gradeLevelIdsT(value)
	            select cast(s as int) from dbo.split(',', @gradeLevelIds)
            end
            if(@classIds is not null)
            begin
	            insert into @classIdsT(value)
	            select cast(s as int) from dbo.split(',', @classIds)
            end

            declare @canInstallForTeahcer bit = @hasTeacherMyApps | @canAttach
            declare @canInstallForStudent bit = @hasStudentMyApps | @canAttach

            declare @canInstall bit = 0
            if (
	            (@callerRoleId = 3 and @hasStudentMyApps = 1) 
	            or (@callerRoleId = 2 and (@canInstallForStudent = 1 or @canInstallForTeahcer = 1))
	            or ((@callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8) 
		             and (@hasAdminMyApps = 1 or @canInstallForStudent = 1 or @canInstallForTeahcer = 1)
	               )
               )
            set @canInstall = 1


            declare @schoolId int = (select SchoolRef from SchoolYear where Id = @schoolYearId)

            declare @preResult table
            (
	            [Type] int not null,
	            GroupId nvarchar(256) not null,
	            PersonId int not null
            );
            declare @localPersons table
            (
	            Id int not null,
	            RoleRef int not null
            );
            --select sp due to security
            if @canInstall = 1
            begin
	            if @callerRoleId = 3
	            begin
		            insert into @localPersons (Id, RoleRef)
		            select PersonRef, RoleRef 
		            from SchoolPerson
		            where PersonRef = @callerId and SchoolRef = @schoolId and @hasStudentMyApps = 1
	            end
	            if  @callerRoleId = 2
	            begin
		            insert into @localPersons (Id, RoleRef)
		            select PersonRef, RoleRef 
		            from SchoolPerson
		            where ((@canInstallForStudent = 1 and PersonRef in (select ClassPerson.PersonRef
															            from ClassPerson
															            join Class on ClassPerson.ClassRef = Class.Id
															            where Class.TeacherRef = @callerId
									  					               )
			               ) 
			               or (PersonRef = @callerId and @canInstallForTeahcer = 1))
			               and SchoolRef = @schoolId
	            end
	            if @callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8
	            begin
		            insert into @localPersons (Id, RoleRef)
		            select PersonRef, RoleRef 
		            from SchoolPerson
		            where ((RoleRef = 2 and @canInstallForTeahcer = 1)
				            or ((RoleRef = 5 or RoleRef = 7 or RoleRef = 8) and @hasAdminMyApps = 1)
				            or (RoleRef = 3 and @canInstallForStudent = 1))
			              and SchoolRef = @schoolId
	            end
            end

            delete from @localPersons where Id in (select PersonRef from ApplicationInstall where ApplicationRef = @applicationId and Active = 1)
            /*
            TYPES:
            0 - role
            1 - department
            2 - grade level
            3 - class
            4 - person

            --insert by roles
            if @roleIds is not null
            Insert into @preResult
            ([type], groupId, PersonId)
            select 0, cast(RoleRef as nvarchar(256)), Id
            from @localPersons
            where RoleRef in (select Value from @roleIdsT)
            --insert by departments
            if @departmentIds is not null
            begin
	            Insert into @preResult ([type], groupId, PersonId)
	            select distinct 1, cast(d.Value as nvarchar(256)), sp.Id
	            from @localPersons as sp
	            join ClassPerson as csp on sp.Id = csp.PersonRef
	            join Class c on csp.ClassRef = c.Id
	            join @departmentIdsT d on c.ChalkableDepartmentRef = d.value


	            Insert into @preResult ([type], groupId, PersonId)
	            select distinct 1, cast(d.Value as nvarchar(256)), sp.Id
	            from @localPersons as sp
	            join Class c on c.TeacherRef = sp.Id
	            join @departmentIdsT d on c.ChalkableDepartmentRef = d.value
            end
            --insert by grade level
            if @gradeLevelIds is not null
            begin
	            Insert into @preResult
	            ([type], groupId, PersonId)
	            select distinct 2, cast(gl.Value as nvarchar(256)), sp.Id
	            from @localPersons as sp
	            join StudentSchoolYear as ssy on sp.Id = ssy.StudentRef and ssy.SchoolYearRef = @schoolYearId
	            join @gradeLevelIdsT gl on ssy.GradeLevelRef = gl.value 
	
	            Insert into @preResult
	            ([type], groupId, PersonId)
	            select distinct 2, cast(gl.Value as nvarchar(256)), sp.Id
	            from @localPersons as sp
	            join Class c on sp.Id = c.TeacherRef
	            join @gradeLevelIdsT gl on c.GradeLevelRef = gl.value
            end
            --insert by class
            if @classIds is not null
            begin
	            Insert into @preResult
	            ([type], groupId, PersonId)
	            select distinct 3, cast(cc.Value as nvarchar(256)), sp.Id
	            from @localPersons as sp
	            join ClassPerson csp on csp.PersonRef = sp.Id
	            join @classIdsT cc on csp.ClassRef = cc.value

	            if @callerRoleId <> 2
	            begin
		            Insert into @preResult ([type], groupId, PersonId)
		            select distinct 3, cast(cc.Value as nvarchar(256)), sp.Id
		            from @localPersons as sp
		            join Class c on sp.Id = c.TeacherRef
		            join @classIdsT cc on c.Id = cc.value
	            end
            end

            declare @isSinglePerson bit = 0
            if @personId is null and @callerRoleId = 2
            begin
	            set @personId = @callerId
	            set @isSinglePerson = 1
            end

            --insert by sp
            if @personId is not null
	            Insert into @preResult([type], groupId, PersonId)
	            select 4, cast(Id as nvarchar(256)), Id
	            from @localPersons
	            where id = @personId



            if @roleIds is null and @departmentIds is null and @gradeLevelIds is null 
	            and @classIds is null and (@isSinglePerson = 1 or @personId is null)
	
	            Insert into @preResult
	            ([type], groupId, PersonId)
	            select 4, cast(Id as nvarchar(256)), Id
	            from @localPersons

            select * 
            from @preResult*/
            throw new NotImplementedException();
        }

        public IList<ApplicationInstall> GetAll(AndQueryCondition personId)
        {

            /*  var ps = new AndQueryCondition
                    {
                        {ApplicationInstall.APPLICATION_REF_FIELD, applicationId},
                        {ApplicationInstall.ACTIVE_FIELD, true}
                    };
                if (owners)
                    ps.Add(ApplicationInstall.OWNER_REF_FIELD, personId);
                else
                   ps.Add(ApplicationInstall.PERSON_REF_FIELD, personId);
             * 
             *  var ps = new AndQueryCondition
                    {
                        {ApplicationInstall.APPLICATION_REF_FIELD, applicationId},
                        {ApplicationInstall.PERSON_REF_FIELD, personId},
                        {ApplicationInstall.ACTIVE_FIELD, true}
                    };
             
             */

            throw new NotImplementedException();
        }

        public void Update(ApplicationInstall appInst)
        {
            if (data.ContainsKey(appInst.Id))
                data[appInst.Id] = appInst;

        }

        public IEnumerable<StudentCountToAppInstallByClass> GetStudentCountToAppInstallByClass(Guid applicationId, int schoolYearId, int userId, int roleId)
        {


            var appInstalls =
                Storage.ApplicationInstallStorage.GetAll().Where(x => x.ApplicationRef == applicationId).ToList();

            /*
             * CREATE PROCEDURE [dbo].[spGetStudentCountToAppInstallByClass]
                @applicationId uniqueidentifier, @schoolYearId int, @personId int, @roleId int
                as

                declare @emptyResult bit = 0;

                select
	                Class.Id as ClassId,
	                Class.Name as ClassName,
	                Count(*) as NotInstalledStudentCount
                from Class
                join ClassPerson on ClassPerson.ClassRef = Class.Id
                left join (select * from ApplicationInstall where ApplicationRef = @applicationId and Active = 1) x
                on x.PersonRef = ClassPerson.PersonRef and x.SchoolYearRef = Class.SchoolYearRef
                where
	                @emptyResult = 0
	                and Class.SchoolYearRef = @schoolYearId
	                and (@roleId = 8 or @roleId = 7 or @roleId = 5 or @roleId = 2 and Class.TeacherRef = @personId)
	                and x.Id is null
                group by
                Class.Id, Class.Name
             */



            throw new NotImplementedException();
        }

        public IList<PersonsForApplicationInstallCount> GetPersonsForApplicationInstallCount(Guid applicationId, int userId, int? personId, IList<int> roleIds, IList<Guid> departmentIds, IList<int> gradeLevelIds, IList<int> classIds, int id, bool hasAdminMyApps, bool hasTeacherMyApps, bool hasStudentMyApps, bool canAttach, int id1)
        {
            /*
             * CREATE PROCEDURE [dbo].[spGetPersonsForApplicationInstallCount]
	                @applicationId uniqueidentifier, @callerId int, @personId int,
	                 @roleIds nvarchar(2048), @departmentIds nvarchar(2048)
	                , @gradeLevelIds nvarchar(2048), @classIds nvarchar(2048), @callerRoleId int
	                , @hasAdminMyApps bit, @hasTeacherMyApps bit, @hasStudentMyApps bit, @canAttach bit, @schoolYearId int
                as
                PRINT('start');
                PRINT(getDate());
                declare @preResult table
                (
	                [Type] int not null,
	                GroupId nvarchar(256) not null,
	                PersonId int not null
                );
                insert into @preResult
                exec dbo.spGetPersonsForApplicationInstall @applicationId, @callerId, @personId, @roleIds, @departmentIds, @gradeLevelIds, @classIds
                , @callerRoleId , @hasAdminMyApps , @hasTeacherMyApps , @hasStudentMyApps , @canAttach, @schoolYearId

                select [Type], [GroupId], Count(*) as [Count]
                from @preResult
                group by [Type], [GroupId]
                union
                select 5 as [Type], null as [GroupId], COUNT(*) as [Count] from (select distinct PersonId from @preResult) z

                PRINT('end');
                PRINT(getDate());


                GO
             */
            throw new NotImplementedException();
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
                data.Where(x => x.Value.OwnerRef == personId || x.Value.PersonRef == personId && x.Value.Active)
                    .Select(x => x.Value)
                    .ToList();

        }
    }
}
