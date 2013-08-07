create function [dbo].[fnCalcStudentGradeAvgForFinalGrade] (@studentId uniqueidentifier, @finalGradeId uniqueidentifier, @date datetime2)
returns int
as 
begin
	declare @avg int =
	( 
	    select Sum(x.AvgByType) 
		from (select  (avg(cast(sa.GradeValue as decimal)) * cast(fgat.PercentValue as decimal) / 100) as AvgByType
			from StudentAnnouncement sa
			join Announcement a on a.Id = sa.AnnouncementRef
			join FinalGradeAnnouncementType fgat on fgat.FinalGradeRef = a.MarkingPeriodClassRef and fgat.AnnouncementTypeRef = a.AnnouncementTypeRef
			join ClassPerson cp on cp.Id = sa.ClassPersonRef
			where a.MarkingPeriodClassRef = @finalGradeId and cp.PersonRef = @studentId
				 and sa.GradeValue is not null and sa.[State] = 2 and fgat.PercentValue > 0
				 and a.Dropped = 0 and sa.Dropped = 0 and a.[State] = 1
				 and (@date is null or a.Expires <= @date)
			group by fgat.Id, fgat.PercentValue) x
	)
	return(@avg); 
end
GO

create function fnCaclStudentAvgByAnnType (@studentId uniqueidentifier, @markingPeriodClassId uniqueidentifier, @annTypeId int, @date datetime2)
returns int
as
begin
	declare @avg int = 
			(
				select avg(sa.GradeValue)
				from StudentAnnouncement sa
				join Announcement a on a.Id = sa.AnnouncementRef
				join ClassPerson cp on cp.Id = sa.ClassPersonRef
				where cp.PersonRef = @studentId
				and a.AnnouncementTypeRef = @annTypeId and a.MarkingPeriodClassRef = @markingPeriodClassId
				and sa.GradeValue is not null and sa.[State] = 2
				and a.Dropped = 0 and sa.Dropped = 0 and a.[State] = 1
				and (@date is null or a.Expires <= @date)
			)
	return (@avg)
end
go
create procedure spCalcStudentSummaryGradeStatsPerDate @markingPeriodId uniqueidentifier, @studentId uniqueidentifier, @classId uniqueidentifier, @dayInterval int
as

---get days
declare @dates table( id uniqueidentifier, [date] datetime2, MarkingPeriodId uniqueidentifier)
insert into @dates
select x.Id, x.[DateTime], x.MarkingPeriodRef
	from(
		select Id, [DateTime], MarkingPeriodRef, row_number() over (order by [DateTime]) as RowNumber
		from [Date]
		where MarkingPeriodRef = @markingPeriodId
	)x
where (x.[RowNumber] % @dayInterval) = 0 or x.[RowNumber] = 1

declare @mpClasses table
(
	Id uniqueidentifier,
	ClassRef uniqueidentifier,
	MarkingPeriodRef uniqueidentifier
	
)
declare @gradeLevelId  uniqueidentifier = (select top 1 GradeLevelRef from StudentInfo where Id = @studentId)

insert into @mpClasses
select MarkingPeriodClass.* from MarkingPeriodClass
join Class on Class.Id = MarkingPeriodClass.ClassRef
where MarkingPeriodClass.MarkingPeriodRef = @markingPeriodId
	  and Class.GradeLevelRef = @gradeLevelId
	  and (@classId is null or Class.Id = @classId)

-- calc student avg
declare @studentAvg table(dateId uniqueidentifier,  [date] datetime2, [avg] int)
insert into @studentAvg
select d.id as dateId,  d.[date] as [date],
	   Avg(dbo.fnCalcStudentGradeAvgForFinalGrade(@studentId, mpc.id, d.[Date]))  as [avg]
from @dates d
join @mpClasses mpc on mpc.MarkingPeriodRef = d.MarkingPeriodId
where exists (select * from ClassPerson where mpc.ClassRef = mpc.ClassRef)
group by d.id, d.[date]

-- calc peers avg
declare @peersAvgPerDate table(dateId uniqueidentifier,  [date] datetime2, [avg] int)
insert into @peersAvgPerDate
select x.id, x.[date], avg(x.[avg])
from 
	(
	  select d.id, d.[date], cp.PersonRef,
			 Avg(dbo.fnCalcStudentGradeAvgForFinalGrade(@studentId, mpc.id, d.[Date])) as [avg]
	  from @dates d
	  join @mpClasses mpc on mpc.MarkingPeriodRef = d.MarkingPeriodId
	  join ClassPerson cp on cp.ClassRef = mpc.ClassRef
	  where cp.PersonRef <> @studentId
	  group by d.id, d.[date], cp.PersonRef
	) x 
group by x.id, x.[date]

---- res
select stAvg.[date] as [Date], 
	   stAvg.[avg] as [Avg], 
	   peersAvg.[avg] as [PeersAvg]
from @studentAvg stAvg
join @peersAvgPerDate peersAvg on peersAvg.dateId = stAvg.dateId
go
create procedure spCalcStudentClassGradeStatsPerDate 
@markingPeriodId uniqueidentifier, @classId uniqueidentifier, @studentId uniqueidentifier, @dayInterval int
as

---get days
declare @dates table( id uniqueidentifier, [date] datetime2, MarkingPeriodId uniqueidentifier)
insert into @dates
select x.Id, x.[DateTime], x.MarkingPeriodRef
	from(
		select Id, [DateTime], MarkingPeriodRef, row_number() over (order by [DateTime]) as RowNumber
		from [Date]
		where MarkingPeriodRef = @markingPeriodId
	)x
where (x.[RowNumber] % @dayInterval) = 0 or x.[RowNumber] = 1

select d.[date] as [Date], cp.PersonRef as StudentId,
	   avg(dbo.fnCalcStudentGradeAvgForFinalGrade(cp.PersonRef, mpc.Id, d.[date])) as [Avg]
from @dates d 
join MarkingPeriodClass mpc on mpc.MarkingPeriodRef = d.MarkingPeriodId
join ClassPerson cp on cp.ClassRef = mpc.ClassRef
where mpc.MarkingPeriodRef = @markingPeriodId and mpc.ClassRef = @classId
	  and(@studentId is null or cp.PersonRef = @studentId)
group by d.[date], cp.PersonRef

select d.[date] as [Date], cp.PersonRef as StudentId,
	   fgat.AnnouncementTypeRef as AnnouncementTypeId,  	   
	   dbo.fnCaclStudentAvgByAnnType(cp.PersonRef, mpc.Id, fgat.AnnouncementTypeRef, d.[date]) as [Avg]
from @dates d
join MarkingPeriodClass mpc on mpc.MarkingPeriodRef = d.MarkingPeriodId
join ClassPerson cp on cp.ClassRef = mpc.ClassRef
join FinalGradeAnnouncementType fgat on fgat.FinalGradeRef = mpc.Id
where mpc.MarkingPeriodRef = @markingPeriodId and mpc.ClassRef = @classId and fgat.PercentValue > 0  
	  and (@studentId is null or cp.PersonRef = @studentId)