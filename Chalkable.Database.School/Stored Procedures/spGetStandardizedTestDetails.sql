CREATE Procedure [dbo].[spGetStandardizedTestDetails]
AS

declare @standardizedTest TStandardizedTest

insert into @standardizedTest
output Inserted.*
select * from StandardizedTest

Select 
	stc.*
From 
	StandardizedTestComponent stc
	join @standardizedTest st 
		on st.Id = stc.StandardizedTestRef

Select 
	stst.*
From 
	StandardizedTestScoreType stst
	join @standardizedTest st 
		on st.Id = stst.StandardizedTestRef

GO