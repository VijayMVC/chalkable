create view [dbo].[vwClassDiscipline]
as
select 	
	vwPerson.*,
	ClassPerson.ClassRef as ClassPerson_ClassRef,
	ClassPerson.Id as ClassPerson_Id,
	ClassPerson.PersonRef as ClassPerson_PersonRef,
	ClassDiscipline.Id as ClassDiscipline_Id,
	ClassDiscipline.[Description] as ClassDiscipline_Description,
	ClassDiscipline.ClassPersonRef as ClassDiscipline_ClassPersonRef,
	ClassDiscipline.ClassPeriodRef as ClassDiscipline_ClassPeriodRef,
	ClassDiscipline.[Date] as ClassDiscipline_Date,
	ClassDisciplineType.Id as ClassDisciplineType_Id,
	DisciplineType.Id as DisciplineType_Id,
	DisciplineType.Name as DisciplineType_Name,
	DisciplineType.Score as DisciplineType_Score,
	Period.Id as Period_Id,
	Period.[Order] as Period_Order,
	Period.MarkingPeriodRef as Period_MarkingPeriodRef,
	Period.StartTime as Period_StartTime,
	Period.EndTime as Period_EndTime,
	Period.SectionRef as Period_SectionRef,
	Class.TeacherRef as Class_TeacherRef,
	Class.SchoolYearRef as Class_SchoolYearRef,
	Class.Name as Class_Name,
	Class.GradeLevelRef as Class_GradeLevelRef,
	ClassPeriod.Id as ClassPeriod_Id

from 
	vwPerson
	join ClassPerson on vwPerson.Id = ClassPerson.PersonRef
	join ClassDiscipline on ClassDiscipline.ClassPersonRef = ClassPerson.Id
	join ClassPeriod on ClassPeriod.Id = ClassDiscipline.ClassPeriodRef
	join Period on ClassPeriod.PeriodRef = Period.Id
	join Class on ClassPerson.ClassRef = Class.Id
	join ClassDisciplineType on ClassDisciplineType.ClassDisciplineRef = ClassDiscipline.Id
	join DisciplineType on DisciplineType.Id = ClassDisciplineType.DisciplineTypeRef
GO

create procedure [dbo].[spGetClassDisciplines] @studentId uniqueidentifier, @markingPeriodId uniqueidentifier, @fromDate DateTime2,
	@toDate DateTime2, @classId uniqueidentifier, @teacherId uniqueidentifier, @type uniqueidentifier, @fromTime int, @toTime int, 
	@schoolYearId uniqueidentifier, @id uniqueidentifier, @needAllData bit
as


if(@needAllData = 0)
begin 
	select 	* 
	from vwClassDiscipline
	where
	(@studentId is null or ClassPerson_PersonRef = @studentId)
	and (@markingPeriodId is null or Period_MarkingPeriodRef = @markingPeriodId)
	and (@fromDate is null or [ClassDiscipline_Date] >= @fromDate)
	and (@toDate is null or [ClassDiscipline_Date] <= @toDate)
	and (@classId is null or ClassPerson_ClassRef = @classId)
	and (@teacherId is null or Class_TeacherRef = @teacherId)	
	and (@type is null or DisciplineType_Id = @type)
	and (@fromTime is null or [Period_StartTime] >= @fromTime)
	and (@toTime is null or [Period_EndTime] <= @toTime)
	and (@schoolYearId is null or Class_SchoolYearRef = @schoolYearId)
	and (@id is null or ClassDiscipline_Id = @id)
end
else 
begin
	declare @emptyId uniqueidentifier = cast(cast(0 as binary) as uniqueidentifier)

	select vwPerson.*,
	ClassPerson.ClassRef as ClassPerson_ClassRef,
	ClassPerson.Id as ClassPerson_Id,
	ClassPerson.PersonRef as ClassPerson_PersonRef,
	Period.Id as Period_Id,
	Period.[Order] as Period_Order,
	Period.MarkingPeriodRef as Period_MarkingPeriodRef,
	Period.StartTime as Period_StartTime,
	Period.EndTime as Period_EndTime,
	Period.SectionRef as Period_SectionRef,
	Class.TeacherRef as Class_TeacherRef,
	Class.SchoolYearRef as Class_SchoolYearRef,
	Class.Name as Class_Name,
	Class.GradeLevelRef as Class_GradeLevelRef,
	ClassPeriod.Id as ClassPeriod_Id,

	case when ClassDiscipline.Id is not null then ClassDiscipline.Id else @emptyId end  as ClassDiscipline_Id,
	ClassDiscipline.Description as ClassDiscipline_Description,
	ClassPerson.Id as ClassDiscipline_ClassPersonRef,
	ClassPeriod.Id as ClassPeriod_ClassPeriodRef,
	case when ClassDisciplineType.Id is not null then DisciplineType.Score else -1 end  as DisciplineType_Score,
	case when ClassDisciplineType.Id is not null then DisciplineType.Id else @emptyId end  as DisciplineType_Id,
	case when ClassDisciplineType.Id is not null then ClassDisciplineType.Id else @emptyId end  as ClassDisciplineType_Id,

	DisciplineType.Name as DisciplineType_Name,
	[Date].[DateTime] as [Date]
	
	from vwPerson
	join ClassPerson on vwPerson.Id = ClassPerson.PersonRef
	join ClassPeriod on ClassPerson.ClassRef = ClassPeriod.ClassRef
	join Period on Period.Id = ClassPeriod.PeriodRef
	join [Date] on [Date].ScheduleSectionRef = Period.SectionRef
	join Class on ClassPerson.ClassRef = Class.Id
	left join ClassDiscipline on ClassDiscipline.ClassPersonRef = ClassPerson.Id 
							  and ClassDiscipline.ClassPeriodRef = ClassPeriod.Id
							  and ClassDiscipline.[Date] = [Date].[DateTime]
	left join ClassDisciplineType on ClassDiscipline.Id = ClassDisciplineType.ClassDisciplineRef
	left join DisciplineType on DisciplineType.Id = ClassDisciplineType.DisciplineTypeRef
	
	where   (@studentId is null or vwPerson.Id = @studentId)
			and (@markingPeriodId is null or Period.MarkingPeriodRef = @markingPeriodId)
			and (@fromDate is null or [Date].[DateTime] >= @fromDate)
			and (@toDate is null or [Date].[DateTime] <= @toDate)
			and (@classId is null or Class.Id = @classId)
			and (@teacherId is null or Class.TeacherRef = @teacherId)	
			and (@fromTime is null or [StartTime] >= @fromTime)
			and (@toTime is null or [EndTime] <= @toTime)
			and (@schoolYearId is null or Class.SchoolYearRef = @schoolYearId)
			
			and (@id is null or (ClassDiscipline.Id is not null and  ClassDiscipline.Id = @id))
			and (@type is null or (ClassDiscipline.Id is not null 
						and DisciplineType.Id = @type))
			
end

GO


