alter function [dbo].[fnCalcStudentGradeAvgForFinalGrade] (@studentId uniqueidentifier, @finalGradeId uniqueidentifier, @date datetime2)
returns int
as 
begin
	declare @avg int =
	( 
	    select  Sum(case when x.AvgByType is not null then x.AvgByType else FinalGradeAnnouncementType.PercentValue end)
		from (select  (avg(cast(sa.GradeValue as decimal)) * cast(fgat.PercentValue as decimal) / 100) as AvgByType,
				fgat.Id as FinalGradeAnnTypeId, 
				fgat.PercentValue as FinalGradeAnnTypePercent
			from StudentAnnouncement sa
			join Announcement a on a.Id = sa.AnnouncementRef
			join FinalGradeAnnouncementType fgat on fgat.FinalGradeRef = a.MarkingPeriodClassRef and fgat.AnnouncementTypeRef = a.AnnouncementTypeRef
			join ClassPerson cp on cp.Id = sa.ClassPersonRef
			where a.MarkingPeriodClassRef = @finalGradeId and cp.PersonRef = @studentId
				 and sa.GradeValue is not null and sa.[State] = 2 and fgat.PercentValue > 0
				 and a.Dropped = 0 and sa.Dropped = 0 and a.[State] = 1
				 and (@date is null or a.Expires <= @date)
			group by fgat.Id, fgat.PercentValue
			) x
		right join FinalGradeAnnouncementType on FinalGradeAnnouncementType.Id = x.FinalGradeAnnTypeId	
		where FinalGradeAnnouncementType.PercentValue > 0 and FinalGradeAnnouncementType.FinalGradeRef = @finalGradeId
	)
	return(@avg); 
end
GO
