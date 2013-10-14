create function fnGetStudentGradeAvgForClass (@studentId uniqueidentifier, @classId uniqueidentifier, @markingPeriodId uniqueidentifier)
returns int 
as
begin
	declare @markingPeriodClassId uniqueidentifier = (select top 1 Id from MarkingPeriodClass 
													  where ClassRef = @classId and MarkingPeriodRef = @markingPeriodId)
	declare @avg int =
	( 
	    select Sum(x.AvgByType) 
		from (select  (avg(cast(sa.GradeValue as decimal)) * cast(fgat.PercentValue as decimal) / 100) as AvgByType
			from StudentAnnouncement sa
			join Announcement a on a.Id = sa.AnnouncementRef
			join FinalGradeAnnouncementType fgat on fgat.FinalGradeRef = a.MarkingPeriodClassRef and fgat.AnnouncementTypeRef = a.AnnouncementTypeRef
			join ClassPerson cp on cp.Id = sa.ClassPersonRef
			where a.MarkingPeriodClassRef = @markingPeriodClassId and cp.PersonRef = @studentId
				 and sa.GradeValue is not null and sa.[State] = 2 and fgat.PercentValue > 0
				 and a.Dropped = 0 and sa.Dropped = 0
			group by fgat.Id, fgat.PercentValue) x
	)
	return(@avg); 
end;
go
