

CREATE Procedure [dbo].[spGetGradingPeriods]
@id int,
@schoolYearId int,
@classId int
As

Select
GradingPeriod.*
From
GradingPeriod
Where
(@id is null or GradingPeriod.Id = @id)
and (@schoolYearId is null or GradingPeriod.SchoolYearRef = @schoolYearid)
and (@classId is null or GradingPeriod.MarkingPeriodRef in (Select MarkingPeriodRef from MarkingPeriodClass where MarkingPeriodClass.ClassRef = @classId))

Order By
GradingPeriod.StartDate