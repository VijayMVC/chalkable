Alter Table Class
	Drop FK_Class_GradeLevel
Alter Table Class
	Drop Column GradeLevelRef
Alter Table Class
	Add MinGradeLevelRef int Constraint FK_Class_MinGradeLevel Foreign Key References GradeLevel(Id)
Alter Table Class
	Add MaxGradeLevelRef int Constraint FK_Class_MaxGradeLevel Foreign Key References GradeLevel(Id)

Alter Table Class
	Drop FK_Class_School
Alter Table Class
	Drop Column SchoolRef

Drop Type TClass

CREATE TYPE TClass AS TABLE(
	[Id] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](1024) NULL,
	[ChalkableDepartmentRef] [uniqueidentifier] NULL,
	[SchoolYearRef] [int] NULL,
	[PrimaryTeacherRef] [int] NULL,
	[MinGradeLevelRef] [int] NULL,
	[MaxGradeLevelRef] [int] NULL,
	[RoomRef] [int] NULL,
	[CourseRef] [int] NULL,
	[GradingScaleRef] [int] NULL,
	[ClassNumber] [nvarchar](41) NULL
)

GO

Alter Procedure [dbo].[spGetSchedule]
	@schoolYearId int,
	@teacherId int,
	@studentId int,
	@classId int,
	@callerId int,
	@from DateTime2,
	@to DateTime2
as

declare @mpId int
set @mpId = (select top 1 id from MarkingPeriod where StartDate <= @to and SchoolYearRef = @schoolYearId order by StartDate desc)


select
	[Date].[Day],
	[Date].IsSchoolDay,
	[Date].DayTypeRef as DayTypeId,
	[Date].SchoolYearRef as SchoolYearId,
	Period.Id as PeriodId,
	Period.[Order] as PeriodOrder,
	Period.Name as PeriodName,
	C.Id as ClassId,
	C.Name as ClassName,
	C.Description as ClassDescription,
	C.ClassNumber,
	C.MinGradeLevelRef as MinGradeLevelId,
	C.MaxGradeLevelRef as MaxGradeLevelId,
	C.ChalkableDepartmentRef as ChalkableDepartmentId,
	Room.Id as RoomId,
	Room.RoomNumber,
	IsNull (V.StartTime, ScheduledTimeSlot.StartTime) as StartTime,
	IsNull (V.EndTime, ScheduledTimeSlot.EndTime) as EndTime,
	cast ((case when @callerId = C.PrimaryTeacherRef then 1 else 0 end) as bit) as CanCreateItem,
	C.PrimaryTeacherRef as TeacherId,
	C.FirstName as TeacherFirstName,
	C.LastName as TeacherLastName,
	C.Gender as TeacherGender
from
	[Date]
	cross join Period	
	left join	(
		Select 			
			Class.Id as Id,
			ClassPeriod.PeriodRef as PeriodRef,
			ClassPeriod.DayTypeRef as DayTypeRef,
			Class.PrimaryTeacherRef as PrimaryTeacherRef,
			Class.RoomRef as RoomRef,
			Class.Name as Name,
			Class.Description as Description,
			Class.ClassNumber as ClassNumber,
			Class.MinGradeLevelRef as MinGradeLevelRef,
			Class.MaxGradeLevelRef as MaxGradeLevelRef,
			Class.ChalkableDepartmentRef as ChalkableDepartmentRef,
			Staff.FirstName as FirstName,
			Staff.LastName as LastName,
			Staff.Gender as Gender
			
		from Class
		join ClassPeriod on Class.Id = ClassPeriod.ClassRef
		join Staff on Class.PrimaryTeacherRef = Staff.Id
		where
		(@classId is null or Class.Id = @classId)
		and (@studentId is null or Class.Id in (select ClassRef from ClassPerson where ClassPerson.PersonRef = @studentId and ClassPerson.MarkingPeriodRef = @mpId))
		and (@teacherId is null or Class.PrimaryTeacherRef = @teacherId 
			and exists(select * from MarkingPeriodClass where MarkingPeriodClass.ClassRef = Class.Id and MarkingPeriodClass.MarkingPeriodRef = @mpId)
					)
		) C on C.PeriodRef = Period.Id and C.DayTypeRef = [Date].DayTypeRef
	left join Room on C.RoomRef = Room.Id
	left join ScheduledTimeSlot on ScheduledTimeSlot.BellScheduleRef = [Date].BellScheduleRef and ScheduledTimeSlot.PeriodRef = Period.Id
	left join 
	(select ClassRef, BellScheduleRef, PeriodRef, StartTime, EndTime
		from SectionTimeSlotVariation
		join ScheduledTimeSlotVariation on SectionTimeSlotVariation.ScheduledTimeSlotVariationRef = ScheduledTimeSlotVariation.Id
		) V on V.ClassRef = C.Id and V.BellScheduleRef = [Date].BellScheduleRef and V.PeriodRef = C.PeriodRef
Where
	[Date].SchoolYearRef = @schoolYearId
	and Period.SchoolYearRef = @schoolYearId
	and [Date].[Day] >= @from
	and [Date].[Day] <= @to

GO


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

declare @canInstallForTeacher bit = @hasTeacherMyApps | @canAttach
declare @canInstallForStudent bit = @hasStudentMyApps | @canAttach

declare @canInstall bit = 0
if (
(@callerRoleId = 3 and @hasStudentMyApps = 1)
or (@callerRoleId = 2 and (@canInstallForStudent = 1 or @canInstallForTeacher = 1))
or ((@callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8)
and (@hasAdminMyApps = 1 or @canInstallForStudent = 1 or @canInstallForTeacher = 1)
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
		select Student.Id, 3
		from Student join UserSchool on UserSchool.UserRef = Student.UserId
		where Student.Id = @callerId and SchoolRef = @schoolId and @hasStudentMyApps = 1
		and exists(select * from StudentSchoolYear where StudentRef = Student.Id and SchoolYearRef = @schoolYearId)
	end
	if  @callerRoleId = 2
	begin
		insert into @localPersons (Id, RoleRef)
		select Student.Id, 3
		from Student join UserSchool on UserSchool.UserRef = Student.UserId
		where (@canInstallForStudent = 1 and Student.Id in (select ClassPerson.PersonRef
		from ClassPerson
		join ClassTeacher on ClassPerson.ClassRef = ClassTeacher.ClassRef
		join Class on Class.Id = ClassTeacher.ClassRef
		where ClassTeacher.PersonRef = @callerId and Class.SchoolYearRef = @schoolYearId
		)
		)
		and SchoolRef = @schoolId
		union
		select Staff.Id, 2
		from Staff join UserSchool on UserSchool.UserRef = Staff.UserId
		where SchoolRef = @schoolId and Staff.Id = @callerId and @canInstallForTeacher = 1
	end
	if @callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8
	begin
		insert into @localPersons (Id, RoleRef)
		select Student.Id, 3 
		from Student join UserSchool on UserSchool.UserRef = Student.UserId
		where @canInstallForStudent = 1 and SchoolRef = @schoolId
		union
		select Staff.Id, 2 
		from Staff join UserSchool on UserSchool.UserRef = Staff.UserId
		where @canInstallForTeacher = 1 and SchoolRef = @schoolId
	end
end

delete from @localPersons where Id in (select PersonRef from ApplicationInstall where ApplicationRef = @applicationId and Active = 1 and SchoolYearRef = @schoolYearId)
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



ALTER VIEW [dbo].[vwClass]
AS
SELECT
	Class.Id as Class_Id,
	Class.ClassNumber as Class_ClassNumber,
	Class.Name as Class_Name,
	Class.Description as Class_Description,
	Class.SchoolYearRef as Class_SchoolYearRef,
	Class.PrimaryTeacherRef as Class_PrimaryTeacherRef,
	Class.MinGradeLevelRef as Class_MinGradeLevelRef,
	Class.MaxGradeLevelRef as Class_MaxGradeLevelRef,
	Class.ChalkableDepartmentRef as Class_ChalkableDepartmentRef,
	Class.CourseRef as Class_CourseRef,
	Class.RoomRef as Class_RoomRef,
	Class.GradingScaleRef as Class_GradingScaleRef,
	Staff.Id as Person_Id,
	Staff.FirstName as Person_FirstName,
	Staff.LastName as Person_LastName,
	Staff.Gender as Person_Gender,
	2 as Person_RoleRef
FROM
	Class
	left join Staff on Staff.Id = Class.PrimaryTeacherRef
where 
	Class.PrimaryTeacherRef is null or Staff.Id is not null
GO




Alter procedure [dbo].[spGetClasses] @schoolYearId int, @markingPeriodId int, @callerId int, @callerRoleId int,
@personId int, @start int, @count int, @classId int,
@filter1 nvarchar(max), @filter2 nvarchar(max), @filter3 nvarchar(max)
as

declare @callerSchoolId int
declare @roleId int

if exists(select * from Student where Id = @personId)
begin
	set @roleId = 3
end
else if  exists(select * from Staff where Id = @personId)
begin
	set @roleId = 2
end

declare @class table
(
	Class_Id int,
	Class_ClassNumber nvarchar(max),
	Class_Name nvarchar(max),
	Class_Description  nvarchar(max),
	Class_SchoolYearRef int,
	Class_PrimaryTeacherRef int,
	Class_MinGradeLevelRef int,
	Class_MaxGradeLevelRef int,
	Class_ChalkableDepartmentRef uniqueidentifier,	
	Class_CourseRef int,
	Class_RoomRef int,
	Class_GradingScaleRef int,
	Person_Id int,
	Person_FirstName nvarchar(max),
	Person_LastName nvarchar(max),
	Person_Gender nvarchar(max),
	Person_RoleRef int,
	Class_StudentsCount int
)

insert into @class
select vwClass.*, (select count(*) from ClassPerson where ClassRef = Class_Id) as Class_StudentsCount
from vwClass
where 
	(@classId is null or Class_Id = @classId)
	and (@schoolYearId is null or Class_SchoolYearRef = @schoolYearId)
	and (@markingPeriodId is null or exists(select * from MarkingPeriodClass where ClassRef = Class_Id and MarkingPeriodRef = @markingPeriodId))

	and (@callerRoleId = 2 or (@callerRoleId = 3 and  exists(select * from ClassPerson where ClassRef = Class_Id and PersonRef = @callerId)))
	and (@personId is null or (@roleId = 2 and exists(select * from ClassTeacher where ClassRef = Class_Id and PersonRef = @personId))
		or (@roleId = 3 and  exists(select * from ClassPerson where ClassRef = Class_Id and PersonRef = @personId))
	)
	and
	(
	(@filter1 is null and @filter2 is null and @filter3 is null) or
	(@filter1 is not null and Class_Name like @filter1 or
	@filter2 is not null and Class_Name like @filter2 or
	@filter3 is not null and Class_Name like @filter3)
	)


select count(*) as SourceCount from @class

select * from @class
	order by Class_Id
	OFFSET @start ROWS FETCH NEXT @count ROWS ONLY


select mpc.* 
from MarkingPeriodClass mpc
	join @class c on c.Class_Id = mpc.ClassRef
GO


