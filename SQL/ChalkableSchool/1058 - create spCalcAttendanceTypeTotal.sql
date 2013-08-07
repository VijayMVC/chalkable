ALTER view [dbo].[vwClassAttendance]
as
select 	
	vwPerson.*,
	ClassPerson.ClassRef as ClassPerson_ClassRef,
	ClassPerson.Id as ClassPerson_Id,
	ClassPerson.PersonRef as ClassPerson_PersonRef,
	ClassAttendance.Id as ClassAttendance_Id,
	ClassAttendance.Type as ClassAttendance_Type,
	ClassAttendance.AttendanceReasonRef as ClassAttendance_AttendanceReasonRef,
	ClassAttendance.[Date] as ClassAttendance_Date,
	Period.Id as Period_Id,
	Period.[Order] as Period_Order,
	Period.MarkingPeriodRef as Period_MarkingPeriodRef,
	Period.StartTime as Period_StartTime,
	Period.EndTime as Period_EndTime,
	Class.TeacherRef as Class_TeacherRef,
	Class.SchoolYearRef as Class_SchoolYearRef,
	Class.Name as Class_Name,
	Class.GradeLevelRef as Class_GradeLevelRef,
	ClassPeriod.Id as ClassPeriod_Id

from 
	vwPerson
	join ClassPerson on vwPerson.Id = ClassPerson.PersonRef
	join ClassAttendance on ClassAttendance.ClassPersonRef = ClassPerson.Id
	join ClassPeriod on ClassPeriod.Id = ClassAttendance.ClassPeriodRef
	join Period on ClassPeriod.PeriodRef = Period.Id
	join Class on ClassPerson.ClassRef = Class.Id

GO
ALTER procedure [dbo].[spGetClassAttendance]
	@studentId uniqueidentifier, @markingPeriodId uniqueidentifier, @fromDate DateTime2, @toDate DateTime2, 
	@classId uniqueidentifier, @teacherId uniqueidentifier, @type int, @classPeriodId uniqueidentifier,
	@fromTime int, @toTime int, @schoolYearId uniqueidentifier, @id uniqueidentifier, @needAllData bit

WITH RECOMPILE
as

declare @a1 bit;
declare @a2 bit;
declare @a4 bit;
declare @a8 bit;
declare @a16 bit;
if (@type is not null and ((1 & @type) <> 0)) set @a1 = 1 else set @a1 = 0;
if (@type is not null and ((2 & @type) <> 0)) set @a2 = 1 else set @a2 = 0;
if (@type is not null and ((4 & @type) <> 0)) set @a4 = 1 else set @a4 = 0;
if (@type is not null and ((8 & @type) <> 0)) set @a8 = 1 else set @a8 = 0;
if (@type is not null and ((16 & @type) <> 0)) set @a16 = 1 else set @a16 = 0;


if(@needAllData = 0)
begin 
	select
		* 
	from 
		vwClassAttendance
	where
		(@studentId is null or Id = @studentId)
		and (@markingPeriodId is null or Period_MarkingPeriodRef = @markingPeriodId)
		and (@fromDate is null or ClassAttendance_Date >= @fromDate)
		and (@toDate is null or ClassAttendance_Date <= @toDate)
		and (@classId is null or ClassPerson_ClassRef = @classId)
		and (@teacherId is null or Class_TeacherRef = @teacherId)		
		and (@fromTime is null or Period_StartTime >= @fromTime)
		and (@toTime is null or Period_EndTime <= @toTime)
		and (@schoolYearId is null or Class_SchoolYearRef = @schoolYearId)
		and (@id is null or ClassAttendance_Id = @id)
		and (@classPeriodId is null or ClassPeriod_Id = @classPeriodId)
		and ((@type is null) 
			or (@a1 = 1 and ClassAttendance_Type = 1)
			or (@a2 = 1 and ClassAttendance_Type = 2)
			or (@a4 = 1 and ClassAttendance_Type = 4)
			or (@a8 = 1 and ClassAttendance_Type = 8)
			or (@a16 = 1 and ClassAttendance_Type = 16)
			)
end
else
begin
	select vwPerson.*,
	ClassPerson.ClassRef as ClassPerson_ClassRef,
	ClassPerson.Id as ClassPerson_Id,
	ClassPerson.PersonRef as ClassPerson_PersonRef,
	case when ClassAttendance.Id is null then 1 else ClassAttendance.Type end  as ClassAttendance_Type,
	ClassAttendance.AttendanceReasonRef as ClassAttendance_AttendanceReasonRef,
	[Date].[DateTime] as ClassAttendance_Date,
	Period.Id as Period_Id,
	Period.[Order] as Period_Order,
	Period.MarkingPeriodRef as Period_MarkingPeriodRef,
	Period.StartTime as Period_StartTime,
	Period.EndTime as Period_EndTime,
	Class.TeacherRef as Class_TeacherRef,
	Class.SchoolYearRef as Class_SchoolYearRef,
	Class.Name as Class_Name,
	Class.GradeLevelRef as Class_GradeLevelRef,
	ClassPeriod.Id as ClassPeriod_Id
	
	from vwPerson
	join ClassPerson on vwPerson.Id = ClassPerson.PersonRef
	join ClassPeriod on ClassPeriod.ClassRef = ClassPerson.ClassRef
	join Period on ClassPeriod.PeriodRef = Period.Id
	join [Date] on [Date].ScheduleSectionRef = Period.SectionRef and [Date].MarkingPeriodRef = Period.MarkingPeriodRef
	join Class on ClassPerson.ClassRef = Class.Id
	join MarkingPeriod on MarkingPeriod.Id = Period.MarkingPeriodRef
	left join ClassAttendance on ClassAttendance.ClassPersonRef = ClassPerson.Id 
							 and ClassAttendance.ClassPeriodRef = ClassPeriod.Id
							 and ClassAttendance.Date = [Date].[DateTime]

	where 
		(@studentId is null or vwPerson.Id = @studentId)
		and (@markingPeriodId is null or MarkingPeriod.Id = @markingPeriodId)
		and (@fromDate is null or [Date].DateTime >= @fromDate)
		and (@toDate is null or [Date].DateTime <= @toDate)
		and (@classId is null or Class.Id = @classId)
		and (@teacherId is null or Class.TeacherRef = @teacherId)		
		and (@fromTime is null or Period.StartTime >= @fromTime)
		and (@toTime is null or Period.EndTime <= @toTime)
		and (@schoolYearId is null or MarkingPeriod.SchoolYearRef = @schoolYearId)
		and (@classPeriodId is null or ClassPeriod.Id = @classPeriodId)

		and(ClassAttendance.Id is null or (
		    (@id is null or ClassAttendance.Id = @id)

			and ((@type is null) 
				or (@a1 = 1 and ClassAttendance.Type = 1)
				or (@a2 = 1 and ClassAttendance.Type = 2)
				or (@a4 = 1 and ClassAttendance.Type = 4)
				or (@a8 = 1 and ClassAttendance.Type = 8)
				or (@a16 = 1 and ClassAttendance.Type = 16)
				)
			))
end
GO


create procedure spCalcAttendanceTypeTotal @markingPeriodId uniqueidentifier, @schoolYearId uniqueidentifier,
 @studentId uniqueidentifier, @fromDate datetime2, @toDate datetime2
as

select 
	  ClassPerson.PersonRef as PersonId,
	  ca.[Type] as AttendanceType,
	  Count(*) as Total
from ClassAttendance ca
join ClassPeriod on ClassPeriod.Id = ca.ClassPeriodRef
join ClassPerson on ClassPerson.Id = ca.ClassPersonRef
join Period on Period.Id = ClassPeriod.PeriodRef
join MarkingPeriod on MarkingPeriod.Id = Period.MarkingPeriodRef
where (@markingPeriodId is null or MarkingPeriod.Id = @markingPeriodId)
	  and (@studentId is null or ClassPerson.PersonRef = @studentId)
	  and (@schoolYearId is null or MarkingPeriod.SchoolYearRef = @schoolYearId)
	  and (@fromDate is null or ca.Date >= @fromDate)
	  and (@toDate is null or ca.Date <= @toDate)
group by ClassPerson.PersonRef, ca.[Type]
go

create procedure spCalcDisciplineTypeTotal @markingPeriodId uniqueidentifier, @schoolYearId uniqueidentifier,
   @studentId uniqueidentifier, @fromDate datetime2, @toDate datetime2
as

select 
	  ClassPerson.PersonRef as PersonId,
	  dt.*,
	  Count(*) as Total
from ClassDisciplineType cdt
join ClassDiscipline cd on cd.Id = cdt.ClassDisciplineRef 
join DisciplineType dt on dt.Id = cdt.DisciplineTypeRef
join ClassPeriod on ClassPeriod.Id = cd.ClassPeriodRef
join ClassPerson on ClassPerson.Id = cd.ClassPersonRef
join Period on Period.Id = ClassPeriod.PeriodRef
join MarkingPeriod on MarkingPeriod.Id = Period.MarkingPeriodRef
where (@markingPeriodId is null or MarkingPeriod.Id = @markingPeriodId)
	  and (@studentId is null or ClassPerson.PersonRef = @studentId)
	  and (@schoolYearId is null or MarkingPeriod.SchoolYearRef = @schoolYearId)
	  and (@fromDate is null or cd.Date >= @fromDate)
	  and (@toDate is null or cd.Date <= @toDate)
group by ClassPerson.PersonRef, dt.Id, dt.Name, dt.Score