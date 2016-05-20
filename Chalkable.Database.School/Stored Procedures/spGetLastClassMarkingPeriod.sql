Create Procedure [dbo].[spGetLastClassMarkingPeriod]
	@classId int,
	@date datetime2
AS

declare @t TMarkingPeriod

insert into @t
	Select Top 1 mp.*
	From
		MarkingPeriod mp join MarkingPeriodClass mpc
			On mp.Id = mpc.MarkingPeriodRef
	Where
		mpc.ClassRef = @classId
		and (mp.EndDate is null or mp.EndDate >= @date)
		and (mp.StartDate is null or mp.StartDate <= @date)
	Order By
		mp.EndDate Desc

declare @resCount int  = (select count(*) from @t)

if (@resCount = 0 and @date is not null)
Begin
	Select Top 1 mp.*
	From
		MarkingPeriod mp join MarkingPeriodClass mpc
			On mp.Id = mpc.MarkingPeriodRef
	Where
		mpc.ClassRef = @classId
	Order By
		mp.EndDate Desc
End Else
	select * from @t