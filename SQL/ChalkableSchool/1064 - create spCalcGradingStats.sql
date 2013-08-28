ALTER function [dbo].[fnCaclStudentAvgByAnnType] (@studentId uniqueidentifier, @markingPeriodClassId uniqueidentifier, @annTypeId int, @date datetime2)
returns int
as
begin
	declare @avg int = 
			(
				select avg(sa.GradeValue)
				from StudentAnnouncement sa
				join Announcement a on a.Id = sa.AnnouncementRef
				join ClassPerson cp on cp.Id = sa.ClassPersonRef
				where 
				(@studentId is null or cp.PersonRef = @studentId)
				and a.AnnouncementTypeRef = @annTypeId and a.MarkingPeriodClassRef = @markingPeriodClassId
				and sa.GradeValue is not null and sa.[State] = 2
				and a.Dropped = 0 and sa.Dropped = 0 and a.[State] = 1
				and (@date is null or a.Expires <= @date)
			)
	return (@avg)
end
GO
create procedure [dbo].[spCalcGradingStats] @callerId uniqueidentifier, @roleId int, @studentId uniqueidentifier, @markingPeriodId uniqueidentifier
as
-- calc avg per class 
declare @classPerson table
(
	Id uniqueidentifier,
	PersonRef uniqueidentifier,
	ClassRef uniqueidentifier,
	StudentAvg int,
	ClassAvg int,
	ClassName nvarchar(max),
	CourseId uniqueidentifier
)
declare @date datetime2 = dateAdd(day, 1, getDate())
insert into @classPerson
select cp.Id, cp.PersonRef, cp.ClassRef,
	   dbo.fnCalcStudentGradeAvgForFinalGrade(cp.PersonRef, mpc.Id, @date),
	   dbo.fnCalcClassGradeAvgPerMP(mpc.Id),
	   Class.Name as ClassName,
	   Class.CourseRef as CourseId
from ClassPerson cp 
join MarkingPeriodClass mpc on mpc.ClassRef = cp.ClassRef
join Class on Class.Id = cp.ClassRef
where cp.PersonRef = @studentId
	  and (@roleId = 1 or @roleId = 2 or @roleId = 5 or @roleId = 7 or @roleId = 8 
	        or (@roleId = 3 and cp.PersonRef  = @callerId))

-- avg per type
declare @annType table
(
	ClassPersonId uniqueidentifier,
	AnnouncementTypeId int,
	AnnouncementTypeName nvarchar(max),
	[StudentItemTypeAvg] int,
	[ClassItemTypeAvg] int,
	MarkingPeriodClassId uniqueidentifier
) 

insert into @annType
select
		cp.Id,
		at.Id as AnnouncementTypeId,
		at.Name as AnnouncementTypeName,
		dbo.fnCaclStudentAvgByAnnType(cp.PersonRef, mpc.Id, at.Id, @date),
		dbo.fnCaclStudentAvgByAnnType(null, mpc.Id, at.Id, @date),
		mpc.Id
from @classPerson cp
join MarkingPeriodClass mpc on mpc.ClassRef = cp.ClassRef
join FinalGradeAnnouncementType fgat on fgat.FinalGradeRef = mpc.Id
join AnnouncementType at on at.Id = fgat.AnnouncementTypeRef
where mpc.MarkingPeriodRef = @markingPeriodId 

-- result 
select 
	cp.*,
	at.AnnouncementTypeId,
	at.AnnouncementTypeName,
	at.MarkingPeriodClassId,
	at.StudentItemTypeAvg,
	at.ClassItemTypeAvg,
	a.Id as AnnouncementId,
	a.[Order] as AnnouncementOrder,
	a.Dropped as AnnouncementDropped,
	(select Avg(sa.GradeValue) from StudentAnnouncement sa where sa.GradeValue is not null and sa.AnnouncementRef = a.Id and sa.[State] = 2) as ItemAvg,
	sa.GradeValue as StudentGrade  
from Announcement a
join StudentAnnouncement sa on sa.AnnouncementRef = a.Id 
join @annType at on at.MarkingPeriodClassId = a.MarkingPeriodClassRef 
				and at.AnnouncementTypeId = a.AnnouncementTypeRef
				and at.ClassPersonId = sa.ClassPersonRef
join @classPerson cp on cp.Id = at.ClassPersonId 
where sa.GradeValue is not null and sa.[State] = 2 and a.[State] = 1

GO


