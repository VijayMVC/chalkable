--Drop Procedure spTeacherSchedule
Create Procedure spGetSchedule
	@schoolYearId int,
	@teacherId int,
	@studentId int,
	@classId int,
	@from DateTime2,
	@to DateTime2
as
select 
	[Date].[Day],
	[Date].IsSchoolDay,
	[Date].DayTypeRef as DayTypeId,
	[Date].SchoolYearRef as SchoolYearId,
	Period.Id as PeriodId,
	Period.[Order] as PeriodOrder,
	C.Id as ClassId,
	C.Name as ClassName,
	C.Description as ClassDescription,
	C.ClassNumber,
	C.GradeLevelRef as GradeLevelId,
	Room.Id as RoomId,
	Room.RoomNumber,
	ScheduledTimeSlot.StartTime,
	ScheduledTimeSlot.EndTime
from 
	[Date]
	cross join Period
	left join ClassPeriod on ClassPeriod.PeriodRef = Period.Id and [Date].DayTypeRef = ClassPeriod.DayTypeRef
	left join 
	(
		Select * from 
			Class
		where 
			(@classId is null or Class.Id = @classId)
			and (@studentId is null or Class.Id in (select ClassRef from ClassStudent where ClassStudent.PersonRef = @studentId))
			and (@teacherId is null or Class.Id in (select ClassRef from ClassTeacher where ClassTeacher.PersonRef = @teacherId))
	) C on ClassPeriod.ClassRef = C.Id
	left join Room on Class.RoomRef = Room.Id
	left join ScheduledTimeSlot on ScheduledTimeSlot.BellScheduleRef = [Date].BellScheduleRef and ScheduledTimeSlot.PeriodRef = Period.Id	
Where
	[Date].SchoolYearRef = @schoolYearId
	and Period.SchoolYearRef = @schoolYearId
	and [Date].[Day] >= @from 
	and [Date].[Day] <= @to
