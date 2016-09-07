CREATE Procedure [dbo].[spGetCourses]
	@authorityId UNIQUEIDENTIFIER,
	@documentId  UNIQUEIDENTIFIER, 
	@subjectDocId UNIQUEIDENTIFIER,
	@gradeLevelCode nvarchar(25),
	@forTopics bit
As

Declare @courseIds TGuid
Declare @gradeId int = Cast(
	Case When @gradeLevelCode is null Then null
	When @gradeLevelCode = 'K' Then 0  
	When @gradeLevelCode = 'PK' Then -1
	Else @gradeLevelCode End as int)

if @forTopics = 0
Begin
	Insert Into @courseIds
		Select Distinct CourseRef
		From [Standard]	
		Where
		(@gradeId is null or ((Cast(Case When GradeLevelLoRef = 'K' Then 0  
			  When GradeLevelLoRef = 'PK' Then -1 
			  Else GradeLevelLoRef End as int) <= @gradeId) 
		  ANd 
		 (Cast(Case When GradeLevelHiRef = 'K' Then 0  
			  When GradeLevelHiRef = 'PK' Then -1 
			  Else GradeLevelHiRef End as int) >= @gradeId)))
		And (@authorityId is null or AuthorityRef = @authorityId)
		And (@documentId is null or DocumentRef = @documentId)
		And (@subjectDocId is null or SubjectDocRef = @subjectDocId)
End
Else
Begin
	Insert Into @courseIds
		Select Distinct CourseRef
		From [Topic]	
		Where (@subjectDocId is null or SubjectDocRef = @subjectDocId)
End

Select Distinct * From Course
Where Course.Id in(select * From @courseIds)