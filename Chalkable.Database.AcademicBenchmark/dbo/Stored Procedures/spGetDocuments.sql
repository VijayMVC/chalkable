
CREATE Procedure [dbo].[spGetDocuments]
	@authorityId UNIQUEIDENTIFIER
As

Declare @documentIds TGuid;


Insert Into @documentIds
	Select DocumentRef From [Standard] Where AuthorityRef = @authorityId

Select Distinct * From Document
Where Id in(select * From @documentIds)