Alter PROCEDURE [dbo].[spGetPersonsForApplicationInstall]
@applicationId uniqueidentifier, @callerId int, @personId int, @classIds nvarchar(2048), @callerRoleId int
, @hasAdminMyApps bit, @hasTeacherMyApps bit, @hasStudentMyApps bit, @canAttach bit, @schoolYearId int
as
declare @classIdsT table(value int);


PRINT('application id');
PRINT(@applicationId);
PRINT('caller id');
PRINT(@callerId);
PRINT('person id');
PRINT(@personId );
PRINT('caller role id');
PRINT(@callerRoleId);

if(@classIds is not null)
begin
	insert into @classIdsT(value)
	select cast(s as int) from dbo.split(',', @classIds)
end

declare @canInstallForTeacher bit = @hasTeacherMyApps | @canAttach
declare @canInstallForStudent bit = @hasStudentMyApps | @canAttach

declare @canInstall bit = 0
if (
	(@callerRoleId = 3 and @hasStudentMyApps = 1)
	or (@callerRoleId = 2 and (@canInstallForStudent = 1 or @canInstallForTeacher = 1))
	or (@callerRoleId = 10 and (@hasAdminMyApps = 1 or @canInstallForStudent = 1 or @canInstallForTeacher = 1)))
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
		if @canInstallForStudent = 1
		begin 
			Insert Into @preResult
				([type], groupId, SchoolYearId, PersonId)
			Select Distinct 
				3, Class.Id, ClassPerson.PersonRef, Class.SchoolYearRef
			From
				ClassPerson
				join Class on ClassPerson.ClassRef = Class.id
				join ClassTeacher on Class.Id = ClassTeacher.ClassRef	
			where Class.Id in (select value from @classIdsT)				
		end
		
		if @canInstallForTeacher = 1
		begin 
			Insert Into @preResult
				([type], groupId, SchoolYearId, PersonId)
			Select Distinct 
				3, Class.Id, ClassTeacher.PersonRef, Class.SchoolYearRef
			From
				Class
				join ClassTeacher on Class.Id = ClassTeacher.ClassRef			
			where Class.Id in (select value from @classIdsT)
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

delete from @preResult 
from 
	(select PersonId, SchoolYearId from @preResult) x
	join ApplicationInstall on x.PersonId = ApplicationInstall.PersonRef and x.SchoolYearId = ApplicationInstall.SchoolYearRef
	
select *
from @preResult
GO


