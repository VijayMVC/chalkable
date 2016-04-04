alter PROCEDURE [dbo].[spGetPersonsForApplicationInstall]
	@applicationId uniqueidentifier, @callerId int, @personId int, @roleIds nvarchar(2048), @departmentIds nvarchar(2048)
	, @gradeLevelIds nvarchar(2048), @classIds nvarchar(2048), @callerRoleId int
	, @hasAdminMyApps bit, @hasTeacherMyApps bit, @hasStudentMyApps bit, @canAttach bit, @schoolYearId int
as
declare @roleIdsT table(value int);
declare @departmentIdsT table(value uniqueidentifier);
declare @gradeLevelIdsT table(value int);
declare @classIdsT table(value int);


PRINT('application id');
PRINT(@applicationId);
PRINT('caller id');
PRINT(@callerId);
PRINT('person id');
PRINT(@personId );
PRINT('caller role id');
PRINT(@callerRoleId);

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
			and exists(select * from StudentSchoolYear where StudentRef = PersonRef and SchoolYearRef = @schoolYearId)
	end
	if  @callerRoleId = 2
	begin
		insert into @localPersons (Id, RoleRef)
		select PersonRef, RoleRef 
		from SchoolPerson
		where ((@canInstallForStudent = 1 and PersonRef in (select ClassPerson.PersonRef
															from ClassPerson
															join ClassTeacher on ClassPerson.ClassRef = ClassTeacher.ClassRef
															join Class on Class.Id = ClassTeacher.ClassRef
															where ClassTeacher.PersonRef = @callerId and Class.SchoolYearRef = @schoolYearId
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
*/
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
	where c.SchoolYearRef = @schoolYearId

	Insert into @preResult ([type], groupId, PersonId)
	select distinct 1, cast(d.Value as nvarchar(256)), sp.Id
	from @localPersons as sp
	join ClassTeacher as ct on ct.PersonRef = sp.Id
	join Class c on c.Id = ct.ClassRef
	join @departmentIdsT d on c.ChalkableDepartmentRef = d.value
	where c.SchoolYearRef = @schoolYearId

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
	join ClassTeacher as ct on ct.PersonRef = sp.Id
	join Class c on c.Id = ct.ClassRef
	join @gradeLevelIdsT gl on c.GradeLevelRef = gl.value
	where c.SchoolYearRef = @schoolYearId
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
		join ClassTeacher ct on sp.Id = ct.PersonRef
		join @classIdsT cc on ct.ClassRef = cc.value
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
from @preResult


GO


