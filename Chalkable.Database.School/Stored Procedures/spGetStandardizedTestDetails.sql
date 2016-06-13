CREATE Procedure [dbo].[spGetStandardizedTestDetails]
	@ids TInt32 ReadOnly
AS

Declare @idsCount int;
Set @idsCount = (Select count(*) From @ids)

Declare @standardizedTest TStandardizedTest

Insert Into @standardizedTest
Output Inserted.*
Select * From StandardizedTest
Where (@idsCount = 0 or Id in (Select * From @ids))

Select 
	stc.*
From 
	StandardizedTestComponent stc
	join @standardizedTest st 
		On st.Id = stc.StandardizedTestRef

Select 
	stst.*
From 
	StandardizedTestScoreType stst
	join @standardizedTest st 
		On st.Id = stst.StandardizedTestRef

GO