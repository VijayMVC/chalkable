Alter Procedure [dbo].[spGetPersonsForApplicationInstall] 
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
declare @installForAll bit = 0
if @classIds is null and @personId is null
	set @installForAll = 1

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
		
		-- get all school classes if classIds and personId parameters are not set
		if @installForAll = 1
		begin
		     -- if caller is admin get all class from school
			 if @callerRoleId = 10
				 insert into @classIdsT
				 select Id from Class where SchoolYearRef = @schoolYearId
		
			 -- if caller is teacher get all teacher classes
			 if @callerRoleId = 2
				insert into @classIdsT 
				select Class.Id from Class
				join ClassTeacher on ClassTeacher.ClassRef = Class.Id
				where Class.SchoolYearRef = @schoolYearId and PersonRef = @callerId
		end 
		--filter classes if caller is teahcer 
		else if @callerRoleId = 2
				delete from @classIdsT
				where value not in (select ClassRef from ClassTeacher where PersonRef = @callerId)


		if @canInstallForStudent = 1
		begin 
			Insert Into @preResult
				([type], groupId, SchoolYearId, PersonId)
			Select Distinct 
				3, Class.Id, Class.SchoolYearRef, ClassPerson.PersonRef
			From
				ClassPerson
				join Class on ClassPerson.ClassRef = Class.id				
				join @classIdsT Ids on Ids.value = Class.Id
				where Class.SchoolYearRef = @schoolYearId
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
				where Class.SchoolYearRef = @schoolYearId
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

declare @a int 
select @a = count(*) from @preResult
print(@a)

select 
	x.*
from 
	@preResult x
	left join (select * from ApplicationInstall where ApplicationInstall.Active = 1 and ApplicationInstall.ApplicationRef = @applicationId) A
		on x.PersonId = A.PersonRef and x.SchoolYearId = A.SchoolYearRef
Where 
	A.Id is null


GO


