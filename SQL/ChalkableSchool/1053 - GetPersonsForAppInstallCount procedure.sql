Create procedure [spGetPersonsForApplicationInstallCount]
@applicationId int, @callerId int, @personId int, @roleIds nvarchar(2048), @departmentIds nvarchar(2048)
, @gradeLevelIds nvarchar(2048), @classIds nvarchar(2048), @callerRoleId int
, @hasAdminMyApps bit, @hasTeacherMyApps bit, @hasStudentMyApps bit, @canAttach bit
as
PRINT('start');
PRINT(getDate());
declare @preResult table
(
	[Type] int not null,
	GroupId int not null,
	PersonId int not null
);
insert into @preResult
exec dbo.spGetPersonsForApplicationInstall @applicationId, @callerId, @personId, @roleIds, @departmentIds, @gradeLevelIds, @classIds
	, @callerRoleId , @hasAdminMyApps , @hasTeacherMyApps , @hasStudentMyApps , @canAttach 

select 
	[Type],
	[GroupId],
	Count(*) as [Count]
from 
	@preResult
group by 
	[Type],
	[GroupId]
union
	select 5 as [Type], null as [GroupId], COUNT(*) as [Count] from (select distinct PersonId from @preResult) z
	
PRINT('end');
PRINT(getDate());
GO
