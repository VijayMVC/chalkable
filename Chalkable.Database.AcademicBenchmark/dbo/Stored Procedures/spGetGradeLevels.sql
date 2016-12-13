CREATE Procedure spGetGradeLevels
	@authorityId UNIQUEIDENTIFIER,
	@documentId  UNIQUEIDENTIFIER, 
	@subjectDocId UNIQUEIDENTIFIER
As

Declare @gradeLevelIds As Table(
	[Lo] nvarchar(25),
	[Hi] nvarchar(25)
);

Insert Into @gradeLevelIds
	Select Distinct
	Cast(
	Case When GradeLevelLoRef = 'K' Then 0  
		 When GradeLevelLoRef = 'PK' Then -1 
		 Else GradeLevelLoRef End as int)as [Lo], 
		 Cast(
	Case When GradeLevelHiRef  = 'K' Then 0  
		 When GradeLevelHiRef  = 'PK' Then -1 
		 Else GradeLevelHiRef  End as int)as [Hi]
	From [Standard] 
	Where 
		@authorityId is null or AuthorityRef = @authorityId
		And @documentId is null or DocumentRef = @documentId
		And @subjectDocId is null or SubjectDocRef = @subjectDocId

Select Distinct 
	GradeLevel.Code,
	GradeLevel.[Description],
	GradeLevel.Low,
	GradeLevel.High,
	Cast(
	Case When Code = 'K' Then 0  
		 When Code = 'PK' Then -1 
		 Else Code End as int) as [Order]
From GradeLevel
join @gradeLevelIds as gl on
Cast(
	Case When Code = 'K' Then 0  
		 When Code = 'PK' Then -1 
		 Else Code End as int) >= [Lo]
And Cast(
	Case When Code = 'K' Then 0  
		 When Code = 'PK' Then -1 
		 Else Code End as int) <= [Hi]
Order by [Order]