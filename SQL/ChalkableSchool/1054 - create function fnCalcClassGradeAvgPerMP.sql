create function [dbo].[fnCalcClassGradeAvgPerMP] (@markingPeriodClassId uniqueidentifier)
returns int
as
begin
     declare @avg int = 
		   (select sum(y.[Avg])
	        from (select avg(x.[Avg] * x.PercentValue / 100) as [Avg]	
				  from  
						(select fgat.Id as AnnouncementType,
							   cast(fgat.PercentValue as decimal)  as PercentValue,
						 cast(AVG(sa.GradeValue)  as decimal) as [Avg]			
						 from Announcement a
						 join StudentAnnouncement sa on sa.AnnouncementRef = a.Id
						 join FinalGradeAnnouncementType fgat on fgat.AnnouncementTypeRef = a.AnnouncementTypeRef and fgat.FinalGradeRef = a.MarkingPeriodClassRef
						 where a.MarkingPeriodClassRef = @markingPeriodClassId
							   and sa.GradeValue is not null and sa.[State] = 2 and fgat.PercentValue > 0
							   and a.Dropped = 0 and sa.Dropped = 0
						 group by fgat.Id, fgat.PercentValue, a.Id
						 ) x
			 	   group by x.AnnouncementType, x.PercentValue
				   ) y
			 )
	 return(@avg)
end;
GO


