
CREATE Procedure [dbo].[spGetPersonsForApplicationInstallCount]
	@applicationId uniqueidentifier, 
	@callerId int, 
	@personId int,
	@classIds TInt32 readonly, 
	@callerRoleId int , 
	@hasAdminMyApps bit, 
	@hasTeacherMyApps bit, 
	@hasStudentMyApps bit, 
	@hasAdminExternalAttach bit, 
	@hasStudentExternalAttach bit,
	@hasTeacherExternalAttach bit, 
	@canAttach bit, 
	@schoolYearId int
As
PRINT('start');
PRINT(getDate());

Declare @preResult Table
(
	[Type] int not null,
	GroupId int,
	SchoolYearId int null, 
	PersonId int not null
);

Insert Into @preResult
exec dbo.spGetPersonsForApplicationInstall 
		@applicationId, 
		@callerId, 
		@personId, 
		@classIds, 
		@callerRoleId, 
		@hasAdminMyApps, 
		@hasTeacherMyApps, 
		@hasStudentMyApps, 
		@hasAdminExternalAttach, 
		@hasStudentExternalAttach,
		@hasTeacherExternalAttach, 
		@canAttach, 
		@schoolYearId

select 
	[Type], [GroupId], Count(*) as [Count]
from 
	@preResult
group by 
	[Type], [GroupId]
union
select 
	5 as [Type], 
	null as [GroupId], 
	COUNT(*) as [Count] 
from 
	(select distinct PersonId from @preResult) z

PRINT('end');
PRINT(getDate());
