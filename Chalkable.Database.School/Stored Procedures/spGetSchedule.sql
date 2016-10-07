CREATE Procedure [dbo].[spGetSchedule]
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

declare @teachingClasses TInt32
insert into @teachingClasses
select ClassRef from ClassTeacher where PersonRef = @callerId


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
cast ((case when @callerId = C.PrimaryTeacherRef or exists(select * from @teachingClasses cId where cId.Value = C.Id) then 1 else 0 end) as bit) as CanCreateItem,
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