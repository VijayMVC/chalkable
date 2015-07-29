Create Procedure spGetStartupData
	@schoolYearId int,
	@personId int,
	@roleId int,
	@now Datetime2
as 
declare @schoolId int
set @schoolId = (select SchoolRef from SchoolYear where Id = @schoolYearId)

Select 
	* 
from 
	AlphaGrade
where 
	Schoolref = @schoolId
	and exists(select * from GradingScaleRange where GradingScaleRange.AlphaGradeRef=AlphaGrade.Id)

Select * from AlternateScore

Select * from MarkingPeriod where SchoolYearRef = @schoolYearId

exec spGetGradingPeriods null, @schoolYearId, null

exec spGetPersonDetails @schoolId, @personId


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
	Class_SchoolYearRef = @schoolYearId
	and (
		(@roleId = 2 and exists(select * from ClassTeacher where ClassRef = Class_Id and PersonRef = @personId))
		or (@roleId = 3 and  exists(select * from ClassPerson where ClassRef = Class_Id and PersonRef = @personId and IsEnrolled = 1))
	)

select * from @class

select mpc.* 
from 
	MarkingPeriodClass mpc
	join @class c on c.Class_Id = mpc.ClassRef

if (@roleId = 3) begin
	exec spGetSchedule @schoolYearId, null, @personId, null, @roleId, @now, @now	
end
else begin
	exec spGetSchedule @schoolYearId, @personId, null, null, @roleId, @now, @now
end

select * from SchoolOption where Id = @schoolId

select * from GradingComment where SchoolRef = @schoolId

select * from AttendanceReason 

select 
	count(*) as UnshownNotificationsCount 
from 
	[Notification]
where 
	Shown = 0 and PersonRef = @personId


select 
	AlphaGrade.Id as AlphaGradeId,
	c.Class_Id as ClassId
from 
	AlphaGrade
	join GradingScaleRange on GradingScaleRange.AlphaGradeRef = AlphaGrade.Id
	join @class c on GradingScaleRange.GradingScaleRef = c.Class_GradingScaleRef    
order by 
	GradingScaleRange.HighValue desc

select 
	AlphaGrade.Id as AlphaGradeId,
	c.Class_Id as ClassId
from 
	AlphaGrade
	join GradingScaleRange on GradingScaleRange.AlphaGradeRef = AlphaGrade.Id
	join ClassroomOption on ClassroomOption.StandardsGradingScaleRef = GradingScaleRange.GradingScaleRef
	join @class c on c.Class_Id = ClassroomOption.id
	