create procedure spSetClassAttendance @classPeriodId uniqueidentifier, @date datetime2, @type int, @attendanceReasonId uniqueidentifier
								,@lastModified datetime2, @description nvarchar(1024), @sisId int, @classPersonsIds nvarchar(max)
as

declare @cpIds table 
(
	id uniqueidentifier not null
) 
if @classPersonsIds is not null
begin
	insert into @cpIds
	select cast(s as uniqueidentifier) from dbo.split(',', @classPersonsIds)
end

update ClassAttendance
set Date = @date, Type = @type, ClassPeriodRef = @classPeriodId,
	AttendanceReasonRef = @attendanceReasonId, LastModified = @lastModified,
	[Description] = @description, SisId = @sisId 
from @cpIds cp
where ClassAttendance.ClassPersonRef = cp.id

insert into ClassAttendance(Id, ClassPeriodRef, ClassPersonRef, [Date], [Type], AttendanceReasonRef, LastModified, [Description], SisId)
select NEWID(), @classPeriodId, cp.id, @date, @type, @attendanceReasonId, @lastModified, @description, @sisId
from @cpIds cp
where cp.id not in (select ClassPersonRef from ClassAttendance where ClassAttendance.Id is not null)


select * from ClassAttendance
where [Date] = @date and ClassPeriodRef = @classPeriodId and ClassPersonRef in (select id from @cpIds)

go


create view [dbo].[vwClassAttendance]
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
	ClassAttendance.LastModified as ClassAttendance_LastModified,
	ClassAttendance.[Description] as ClassAttendance_Description,
	ClassAttendance.SisId as ClassAttendance_SisId,
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


create procedure [dbo].[spGetClassAttendance]
	@studentId uniqueidentifier, @markingPeriodId uniqueidentifier, @fromDate DateTime2, @toDate DateTime2, 
	@classId uniqueidentifier, @teacherId uniqueidentifier, @type int,
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
	ClassAttendance.LastModified as ClassAttendance_LastModified,
	ClassAttendance.[Description] as ClassAttendance_Description,
	ClassAttendance.SisId as ClassAttendance_SisId,
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





