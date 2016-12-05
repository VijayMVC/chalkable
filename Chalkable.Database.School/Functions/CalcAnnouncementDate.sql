CREATE FUNCTION [dbo].[CalcAnnouncementDate] (@classDays TDate ReadOnly, @currentDate datetime2, @shift int)
RETURNS DATETIME2
AS
BEGIN

--Need to fix date if it is not class day
If Not Exists(select * from @classDays Where [Day] = @currentDate)
Begin
	Declare @neighborDates table(
		[Day] datetime2,
		[Distance] int
	);

	--Low bound
	Insert Into @neighborDates
	Select Max([Day]), DateDiff(d, Max([Day]), @currentDate) From @classDays
	Where [Day] < @currentDate

	--High bound
	Insert Into @neighborDates
	Select Min([Day]), DateDiff(d, @currentDate, Min([Day])) From @classDays
	Where [Day] > @currentDate

	--Resolving new date.
	Set @currentDate = (Select Top 1 [Day] From @neighborDates Where [Day] is not null Order By Distance, [Day]);
End;

Declare @newDate datetime2;

If @shift = 0
	Set @newDate = @currentDate;
Else If @shift < 0
Begin
	Set @shift = -@shift;

	Set @newDate = (
		Select Min(x.[Day]) From (
			Select Top(@shift) [Day] From @classDays Where [Day] < @currentDate Order By [Day] Desc
		) as x
	)

	--If we have null, it means date is out of school year.
	--This can only be out of school year start date.
	--According to task we set first day of class days.
	Set @newDate = IIF(@newDate is null, (Select Min([Day]) From @classDays), @newDate);
End
Else If @shift > 0
Begin
	Set @newDate = (
		Select Max(x.[Day]) From (
			Select Top(@shift) [Day] From @classDays Where [Day] > @currentDate Order By [Day] ASC
		) as x
	)

	--If we have null, it means date is out of school year.
	--This can only be out of school year end date.
	--According to task we set last day of class days.
	Set @newDate = IIF(@newDate is null, (Select Max([Day]) From @classDays), @newDate);
End;

Return @newDate;

END
GO