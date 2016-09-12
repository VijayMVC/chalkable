CREATE Procedure [dbo].[spGetSubjectDocs]
	@authorityId UNIQUEIDENTIFIER,
	@documentId  UNIQUEIDENTIFIER,
	@forTopics bit
As

Declare @subjectDocIds TGuid;

if @forTopics = 0
	Insert Into @subjectDocIds
		Select SubjectDocRef From [Standard] 
		Where 
			@authorityId is null or AuthorityRef = @authorityId
			And @documentId is null or DocumentRef = @documentId
Else
Insert Into @subjectDocIds
		Select SubjectDocRef From [Topic]

Select Distinct * From SubjectDoc
Where Id in(select * From @subjectDocIds)