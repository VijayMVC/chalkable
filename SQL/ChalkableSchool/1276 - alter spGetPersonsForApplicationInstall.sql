Alter Procedure [dbo].[spGetPersonsForApplicationInstall]
	@applicationId uniqueidentifier, 
	@callerId int, 
	@personId int, 
	@classIds TInt32 readonly, 
	@callerRoleId int, 
	@hasAdminMyApps bit, 
	@hasTeacherMyApps bit, 
	@hasStudentMyApps bit,
	@hasAdminExternalAttach bit, 
	@hasStudentExternalAttach bit,
	@hasTeacherExternalAttach bit, 
	@canAttach bit, 
	@schoolYearId int
as

declare @classIdsT TInt32

PRINT('application id');
PRINT(@applicationId);
PRINT('caller id');
PRINT(@callerId);
PRINT('person id');
PRINT(@personId );
PRINT('caller role id');
PRINT(@callerRoleId);

if exists (select * from @classIds)
begin
insert into @classIdsT(value)
select value from @classIds
end


declare @canInstallForTeacher bit = @hasTeacherMyApps | @canAttach | @hasTeacherExternalAttach
declare @canInstallForStudent bit = @hasStudentMyApps | @canAttach | @hasStudentExternalAttach
declare @canInstallForAdmin bit = @hasAdminMyApps | @hasAdminExternalAttach 

declare @canInstall bit = 0
if (
	(@callerRoleId = 3 and @hasStudentMyApps = 1)
	or (@callerRoleId = 2 and (@canInstallForStudent = 1 or @canInstallForTeacher = 1))
	or (@callerRoleId = 10 and (@canInstallForAdmin = 1 or @canInstallForStudent = 1 or @canInstallForTeacher = 1)))
set @canInstall = 1


declare @preResult table
(
	[Type] int not null,
	GroupId int not null,
	SchoolYearId int not null,
	PersonId int not null
);

--select sp due to security
if @canInstall = 1
begin

	if @callerRoleId = 3
	begin
		If @schoolYearId is null
			Throw 10001, 'School Year should be defined for personal installation', 1;

		If @personId is null or @personId <> @callerId
			Throw 10002, 'Student can install app just for him self', 1;

		Insert into @preResult
		([type], groupId, SchoolYearId, PersonId)
		Select Distinct
		4, @personid, @schoolYearId, @personid
	end
	if  @callerRoleId = 10 or @callerRoleId = 2
	begin
		if @callerRoleId = 2
			delete from @classIdsT
			where not exists(
								Select * 
								From ClassTeacher 
								Join Class on Class.Id = ClassTeacher.ClassRef 
								Where PersonRef = @callerId and Class.SchoolYearRef = @schoolYearId and ClassTeacher.ClassRef = value
							)	

		if @canInstallForStudent = 1
		begin
			Insert Into @preResult
			([type], groupId, SchoolYearId, PersonId)
			Select Distinct 3, Class.Id, Class.SchoolYearRef, ClassPerson.PersonRef
			From ClassPerson
			join Class on ClassPerson.ClassRef = Class.id
			join @classIdsT Ids on Ids.value = Class.Id
		end

		if @canInstallForTeacher = 1
		begin
			Insert Into @preResult
			([type], groupId, SchoolYearId, PersonId)
			Select Distinct
			3, Class.Id, Class.SchoolYearRef, ClassTeacher.PersonRef
			From
			Class
			join ClassTeacher on Class.Id = ClassTeacher.ClassRef
			join @classIdsT Ids on Ids.value = Class.Id
		end

		if @personId is not null and not exists (select * from @preResult where personId = @personId)
		begin
			Insert into @preResult
			([type], groupId, SchoolYearId, PersonId)
			Select Distinct
			4, @personid, @schoolYearId, @personid
		end
	end
end

Declare @a int
Select @a = count(*) From @preResult
Print(@a)

Select
	x.*
From 
	@preResult x
Left join (Select * From ApplicationInstall Where ApplicationInstall.Active = 1 and ApplicationInstall.ApplicationRef = @applicationId) A
	On x.PersonId = A.PersonRef and x.SchoolYearId = A.SchoolYearRef
Where
	A.Id is null

GO


Alter Procedure [dbo].[spGetPersonsForApplicationInstallCount]
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

GO


