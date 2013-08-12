ALTER procedure [dbo].[spGetPersonsForApplicationInstall]
@applicationId uniqueidentifier, @callerId uniqueidentifier, @personId uniqueidentifier, @roleIds nvarchar(2048), @departmentIds nvarchar(2048)
, @gradeLevelIds nvarchar(2048), @classIds nvarchar(2048), @callerRoleId int
, @hasAdminMyApps bit, @hasTeacherMyApps bit, @hasStudentMyApps bit, @canAttach bit
as
declare @roleIdsT table(value int);
declare @departmentIdsT table(value uniqueidentifier);
declare @gradeLevelIdsT table(value uniqueidentifier);
declare @classIdsT table(value uniqueidentifier);


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
select cast(s as int) from dbo.[Split](',', @roleIds) 
end
if(@departmentIds is not null)
begin
insert into @departmentIdsT(value)
select cast(s as uniqueidentifier) from dbo.[Split](',', @departmentIds)
end
if(@gradeLevelIds is not null)
begin
insert into @gradeLevelIdsT(value)
select cast(s as uniqueidentifier) from dbo.[Split](',', @gradeLevelIds)
end
if(@classIds is not null)
begin
insert into @classIdsT(value)
select cast(s as uniqueidentifier) from dbo.[Split](',', @classIds)
end

declare @canInstallForTeahcer bit = @hasTeacherMyApps | @canAttach
declare @canInstallForStudent bit = @hasStudentMyApps | @canAttach

declare @canInstall bit = 0
if ((@callerRoleId = 3 and @hasStudentMyApps = 1) or (@callerRoleId = 2 and (@canInstallForStudent = 1 or @canInstallForTeahcer = 1))
or ((@callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8) and (@hasAdminMyApps = 1 or @canInstallForStudent = 1 or @canInstallForTeahcer = 1)))
set @canInstall = 1



declare @preResult table
(
[Type] int not null,
GroupId nvarchar(256) not null,
personId uniqueidentifier not null
);
declare @localPersons table
(
Id uniqueidentifier not null,
RoleRef int not null
);
--select sp due to security
if @canInstall = 1
begin
if @callerRoleId = 3
begin
insert into @localPersons
(Id, RoleRef)
select Id, RoleRef from
Person
where
Id = @callerId	and @hasStudentMyApps = 1

end
if  @callerRoleId = 2
begin
insert into @localPersons
(Id, RoleRef)
select Id, RoleRef from
Person
where
(@canInstallForStudent = 1 and	Id in (select ClassPerson.PersonRef from
ClassPerson
join Class on ClassPerson.ClassRef = Class.Id
where Class.TeacherRef = @callerId
)
) or (Id = @callerId and @canInstallForTeahcer = 1)
end
if @callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8
begin
insert into @localPersons
(Id, RoleRef)
select Id, RoleRef from
Person
where
(RoleRef = 2 and @canInstallForTeahcer = 1)
or ((RoleRef = 5 or RoleRef = 7 or RoleRef = 8) and @hasAdminMyApps = 1)
or (RoleRef = 3 and @canInstallForStudent = 1)
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
Insert into @preResult
([type], groupId, PersonId)
select distinct 1, cast(d.Value as nvarchar(256)), sp.Id
from @localPersons as sp
join ClassPerson as csp on sp.Id = csp.PersonRef
join Class c on csp.ClassRef = c.Id
join Course ci on ci.Id = c.CourseRef
join @departmentIdsT d on ci.ChalkableDepartmentRef = d.value


Insert into @preResult
([type], groupId, PersonId)
select distinct 1, cast(d.Value as nvarchar(256)), sp.Id
from @localPersons as sp
join Class sc on sc.TeacherRef = sp.Id
join Course ci on ci.Id = sc.CourseRef
join @departmentIdsT d on ci.ChalkableDepartmentRef = d.value
end
--insert by grade level
if @gradeLevelIds is not null
begin
Insert into @preResult
([type], groupId, PersonId)
select distinct 2, cast(gl.Value as nvarchar(256)), sp.Id
from @localPersons as sp
join StudentInfo as si on sp.Id = si.Id
join @gradeLevelIdsT gl on si.GradeLevelRef = gl.value


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
Insert into @preResult
([type], groupId, PersonId)
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
Insert into @preResult
([type], groupId, PersonId)
select 4, cast(Id as nvarchar(256)), Id
from @localPersons
where id = @personId



if @roleIds is null and @departmentIds is null and @gradeLevelIds is null and @classIds is null and (@isSinglePerson = 1 or @personId is null)
Insert into @preResult
([type], groupId, PersonId)
select 4, cast(Id as nvarchar(256)), Id
from @localPersons

select * from @preResult

GO

alter procedure [dbo].[spGetPersonsForApplicationInstallCount]
@applicationId uniqueidentifier, @callerId uniqueidentifier, @personId uniqueidentifier,
 @roleIds nvarchar(2048), @departmentIds nvarchar(2048)
, @gradeLevelIds nvarchar(2048), @classIds nvarchar(2048), @callerRoleId int
, @hasAdminMyApps bit, @hasTeacherMyApps bit, @hasStudentMyApps bit, @canAttach bit
as
PRINT('start');
PRINT(getDate());
declare @preResult table
(
[Type] int not null,
GroupId nvarchar(256) not null,
PersonId uniqueidentifier not null
);
insert into @preResult
exec dbo.spGetPersonsForApplicationInstall @applicationId, @callerId, @personId, @roleIds, @departmentIds, @gradeLevelIds, @classIds
, @callerRoleId , @hasAdminMyApps , @hasTeacherMyApps , @hasStudentMyApps , @canAttach

select [Type], [GroupId], Count(*) as [Count]
from @preResult
group by [Type], [GroupId]
union
select 5 as [Type], null as [GroupId], COUNT(*) as [Count] from (select distinct PersonId from @preResult) z

PRINT('end');
PRINT(getDate());
go