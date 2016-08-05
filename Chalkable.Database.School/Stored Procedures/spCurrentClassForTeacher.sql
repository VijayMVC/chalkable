
CREATE Procedure [dbo].[spCurrentClassForTeacher]
@schoolYearId int,
@teacherId int,
@date datetime2,
@time int
as

declare @bs int, @dt int, @mpId int
set @bs = (select BellScheduleRef from [date] where [date].Day = @date and [date].SchoolYearRef = @schoolYearId)
set @dt = (select DayTypeRef from [date] where [date].Day = @date and [date].SchoolYearRef = @schoolYearId)
set @mpId = (select Id from MarkingPeriod where StartDate <= @date and EndDate >= @date and SchoolYearRef = @schoolYearId)

declare @periodT table (id int)
insert into @periodT
select PeriodRef from ScheduledTimeSlot where BellScheduleRef = @bs and StartTime <= @time and EndTime >= @time

Select Class.*
from
Class
join MarkingPeriodClass on MarkingPeriodClass.ClassRef = Class.Id
join ClassTeacher on Class.Id = ClassTeacher.ClassRef
join ClassPeriod on ClassPeriod.ClassRef = CLass.Id
join @periodT p on p.Id = ClassPeriod.PeriodRef
Where
ClassPeriod.DayTypeRef = @dt
and ClassTeacher.PersonRef = @teacherId
and MarkingPeriodClass.MarkingPeriodRef = @mpId