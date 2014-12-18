Alter Procedure [dbo].[spGetSchedule]
	@schoolYearId int,
	@teacherId int,
	@studentId int,
	@classId int,
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
	C.Id as ClassId,
	C.Name as ClassName,
	C.Description as ClassDescription,
	C.ClassNumber,
	C.GradeLevelRef as GradeLevelId,
	C.ChalkableDepartmentRef as ChalkableDepartmentId,
	Room.Id as RoomId,
	Room.RoomNumber,
	ScheduledTimeSlot.StartTime,
	ScheduledTimeSlot.EndTime
from
	[Date]
	cross join Period	
	left join	(
		Select * from
		Class
		join ClassPeriod on Class.Id = ClassPeriod.ClassRef
		where
		(@classId is null or Class.Id = @classId)
		and (@studentId is null or Class.Id in (select ClassRef from ClassPerson where ClassPerson.PersonRef = @studentId and ClassPerson.MarkingPeriodRef = @mpId))
		and (@teacherId is null or Class.PrimaryTeacherRef = @teacherId 
			and exists(select * from MarkingPeriodClass where MarkingPeriodClass.ClassRef = Class.Id and MarkingPeriodClass.MarkingPeriodRef = @mpId)
					)
		) C on C.PeriodRef = Period.Id and C.DayTypeRef = [Date].DayTypeRef
	left join Room on C.RoomRef = Room.Id
	left join ScheduledTimeSlot on ScheduledTimeSlot.BellScheduleRef = [Date].BellScheduleRef and ScheduledTimeSlot.PeriodRef = Period.Id
Where
	[Date].SchoolYearRef = @schoolYearId
	and Period.SchoolYearRef = @schoolYearId
	and [Date].[Day] >= @from
	and [Date].[Day] <= @to

GO


