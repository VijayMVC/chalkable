Create Procedure [dbo].[spGetStartupData]
	@schoolYearId int,
	@personId int,
	@roleId int,
	@now Datetime2
as
declare @schoolId int
set @schoolId = (select SchoolRef from SchoolYear where Id = @schoolYearId)


exec spGetAlphaGradesBySchool @schoolId

Select * from AlternateScore
Select * from MarkingPeriod where SchoolYearRef = @schoolYearId

exec spGetGradingPeriods null, @schoolYearId, null
exec spGetPersonDetails @schoolId, @personId

Declare @class TClassDetails

insert into @class
select 
	vwClass.*, (select count(*) from ClassPerson where ClassRef = Class_Id) as Class_StudentsCount
from vwClass
where
	Class_SchoolYearRef = @schoolYearId
	and (
	(@roleId = 2 and exists(select * from ClassTeacher where ClassRef = Class_Id and PersonRef = @personId))
	or (@roleId = 3 and  exists(select * from ClassPerson where ClassRef = Class_Id and PersonRef = @personId and IsEnrolled = 1))
	)

exec spSelectClassDetails @class

select * from Room

if (@roleId = 3) begin
exec spGetSchedule @schoolYearId, null, @personId, null, @roleId, @now, @now
end
else begin
exec spGetSchedule @schoolYearId, @personId, null, null, @roleId, @now, @now
end

select * from SchoolOption where Id = @schoolId

select * from GradingComment where SchoolRef = @schoolId

exec spGetAttendanceReasons null

Select
	count(*) as UnshownNotificationsCount
From
	[Notification]
Where
	Shown = 0 And 
	PersonRef = @personId And
	RoleRef = @roleId


Declare @classids TInt32 
Insert Into @classids
Select c.Class_Id From @class c

exec spGetAlphaGradesForClasses @classids

exec spGetAlphaGradesForClassStandards @classIds

Declare @schoolIds TInt32
Insert into @schoolids
values(@schoolId)

exec spGetAlphaGradesForSchoolStandards @schoolIds
