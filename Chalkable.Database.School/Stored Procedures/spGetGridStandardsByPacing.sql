Create Procedure [dbo].[spGetGridStandardsByPacing] @ClassId int, @GradeLavelId int, @GradingPeriodId int, @StandardSubjectId int, @ParentStandardId int, @AllStandards bit, @IsActive bit
AS

declare @GPStartDate datetime2;
declare @GPEndDate datetime2;

Select 	@GPStartDate = StartDate, @GPEndDate = EndDate 
From GradingPeriod Where Id = @GradingPeriodId

declare @StandardsForSorting table
(
	Id int,
	ParentStandardRef int,
	Name nvarchar(100),
	[Description] nvarchar(MAX),
	StandardSubjectRef int,
	LowerGradeLevelRef int,
	UpperGradeLevelRef int,
	IsActive bit,
	AcademicBenchmarkId uniqueidentifier,
	IsDeepest bit
)

declare @MinDateForStandard table
(	
	Id int,
	MinDate datetime2
)
 
insert into @StandardsForSorting
Select 
	[Standard].*,
	cast((case when exists(Select * From  [Standard]  innerSt  Where innerSt.ParentStandardRef = [Standard].Id)  then 0 else 1 end) as bit)
From 
	[Standard]  
Where 
(
	(@GradeLavelId IS NULL OR ([Standard].[LowerGradeLevelRef]<=@GradeLavelId AND [Standard].[UpperGradeLevelRef]>=@GradeLavelId))
	AND
	(@ParentStandardId IS NULL OR [Standard].[ParentStandardRef]=@ParentStandardId)
	AND
	(@StandardSubjectId IS NULL OR [Standard].[StandardSubjectRef]=@StandardSubjectId)
	AND 
	(@IsActive = 0 OR [Standard].[IsActive]=@IsActive)
	AND 
	(@classId IS NULL 
	OR
	[Standard].[Id] IN 
		(select 
			[ClassStandard].[StandardRef] 
		from 
			[ClassStandard]
			join [Class] 
				ON [Class].[Id] = [ClassStandard].[ClassRef] or [Class].[CourseRef] = [ClassStandard].[ClassRef]
		where 
		    [Class].[Id] = @classId AND [Class].[CourseRef] IS NOT NULL
		) 
		AND 
		(
		    (@ParentStandardId IS NOT NULL OR @AllStandards = 1) 
		    OR
			([Standard].[ParentStandardRef] IS NULL 
			or 
			[Standard].[ParentStandardRef] NOT IN 
			    (select 
			    	[ClassStandard].[StandardRef] 
			    from [ClassStandard]
			    join [Class] 
			    	ON [Class].[Id] = [ClassStandard].[ClassRef] or [Class].[CourseRef] = [ClassStandard].[ClassRef]
				where 
			        [Class].[Id] = @classId AND [Class].[CourseRef] IS NOT NULL
				)
			)
		)
	)
)


Insert Into @MinDateForStandard
Select 
	AnnouncementStandard.StandardRef as Id,
	Min(CA.Expires) as  MinDate
From 
	AnnouncementStandard 
Join ClassAnnouncement CA 
		on AnnouncementStandard.AnnouncementRef = CA.Id
Where CA.Expires >= @GPStartDate AND CA.Expires <= @GPEndDate And CA.ClassRef = @ClassId
Group By AnnouncementStandard.StandardRef
 
Declare @MAX_DATETIME datetime2 = cast('9999-12-31 23:59:59.998' as datetime2)
 			
Select MDS.MinDate, SFS.*
From @StandardsForSorting SFS
Left Join @MinDateForStandard MDS on MDS.Id = SFS.Id 
Order By 
	(case when MDS.MinDate is not null then MDS.MinDate else @MAX_DATETIME end), 
	SFS.Name

GO