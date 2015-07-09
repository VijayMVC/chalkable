Alter Table Period
	Add Name nvarchar(20)
GO
Update Period
	Set Name = cast([Order] as nvarchar(20))
GO
Alter Table Period
	Alter Column Name nvarchar(20) not null
GO
DROP TYPE [dbo].[TPeriod]
GO
CREATE TYPE [dbo].[TPeriod] AS TABLE(
	[Id] [int] NOT NULL,
	[SchoolYearRef] [int] NOT NULL,
	[Order] [int] NOT NULL,
	Name nvarchar(20) not null
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
	C.GradeLevelRef as GradeLevelId,
	C.ChalkableDepartmentRef as ChalkableDepartmentId,
	Room.Id as RoomId,
	Room.RoomNumber,
	ScheduledTimeSlot.StartTime,
	ScheduledTimeSlot.EndTime,
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
			Class.GradeLevelRef as GradeLevelRef,
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
Where
	[Date].SchoolYearRef = @schoolYearId
	and Period.SchoolYearRef = @schoolYearId
	and [Date].[Day] >= @from
	and [Date].[Day] <= @to


GO


