Alter Procedure [dbo].[spGetPersonsForApplicationInstallCount]
	@applicationId uniqueidentifier, @callerId int, @personId int,
	 @classIds nvarchar(2048), @callerRoleId int , @hasAdminMyApps bit, @hasTeacherMyApps bit, @hasStudentMyApps bit, @canAttach bit, @schoolYearId int
As
PRINT('start');
PRINT(getDate());

Declare @preResult Table
(
	[Type] int not null,
	GroupId int not null,
	SchoolYearId int null, 
	PersonId int not null
);

Insert Into @preResult
exec dbo.spGetPersonsForApplicationInstall @applicationId, @callerId, @personId, @classIds
, @callerRoleId , @hasAdminMyApps , @hasTeacherMyApps , @hasStudentMyApps , @canAttach, @schoolYearId

select [Type], [GroupId], Count(*) as [Count]
from @preResult
group by [Type], [GroupId]
union
select 5 as [Type], null as [GroupId], COUNT(*) as [Count] from (select distinct PersonId from @preResult) z

PRINT('end');
PRINT(getDate());
GO


