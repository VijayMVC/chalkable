CREATE Procedure [dbo].[spGetGridStandardsByPacing] @ClassId int, @GradeLavelId int = null, @StandardSubjectId int, @ParentStandardId int, @AllStandards bit, @IsActive bit
AS

declare @StandardsForSorting table
(
	Id int,
	ParentStandardRef int,
	Name nvarchar(100),
	Description nvarchar(MAX),
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

insert into @MinDateForStandard
	Select 
		Id,
		ISNULL(Min(SortDate), '9999-12-31 23:59:59.998') MinDate 
	From 
		(
			Select SFS.Id, Min(StartDate) as SortDate 
			From 
				@StandardsForSorting as SFS
				left join AnnouncementStandard 
					on SFS.Id = AnnouncementStandard.standardRef
				left join LessonPlan 
					on AnnouncementStandard.AnnouncementRef = LessonPlan.Id
			Group By SFS.Id

			UNION

			Select SFS.Id, Min(Expires) as SortDate 
			From 
				@StandardsForSorting SFS
				left join AnnouncementStandard 
					on SFS.Id = AnnouncementStandard.standardRef
				left join ClassAnnouncement 
					on AnnouncementStandard.AnnouncementRef = ClassAnnouncement.Id
			Group By SFS.Id

			UNION

			Select SFS.Id, Min(Expires) as SortDate
			From 
				@StandardsForSorting SFS
				left join AnnouncementStandard 
					on SFS.Id = AnnouncementStandard.standardRef
				left join SupplementalAnnouncement 
					on AnnouncementStandard.AnnouncementRef = SupplementalAnnouncement.Id
			Group By SFS.Id
		) as x
	Group By 
		Id
			
select
	* 
from 
	@StandardsForSorting SFS
join (select MDS.Id, MDS.MinDate from @MinDateForStandard MDS) SA
	on SFS.Id = SA.Id
order by 
	MinDate, 
	Name

GO