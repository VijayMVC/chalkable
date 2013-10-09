ALTER procedure [dbo].[spRebuildSections] @markingPeriodIds nvarchar(max), @newSectionNames nvarchar(max)
as

declare @mpIds table (id uniqueidentifier)
declare @sections table(name nvarchar(max))
if(@markingPeriodIds is not null and @newSectionNames is not null)
begin
	insert into @mpIds
	select cast(s as uniqueidentifier) from dbo.split(',', @markingPeriodIds)

	insert into @sections
	select cast(s as nvarchar(max)) from dbo.split(',', @newSectionNames)

	delete from [Date]
	where MarkingPeriodRef in (select Id from @mpIds)

	delete from ClassPeriod
	where PeriodRef in (select Id from Period where MarkingPeriodRef in (select Id from @mpIds))
	delete from Period where MarkingPeriodRef in (select Id from @mpIds)
	delete from ScheduleSection where MarkingPeriodRef in (select Id from @mpIds)

	declare @mpIndex int = 1
	--declare @mpCount int = (select COUNT(*) from @mpIds)
	declare @sectionIndex int = 1
	declare @sectionCount int = (select COUNT(*) from @sections)

	declare @mpId uniqueidentifier, @section nvarchar(max)

	declare mpCursor cursor for
	select id from @mpIds

	open mpCursor
	fetch next from mpCursor
	into @mpId

	declare sectionC scroll cursor for
	select name from @sections

	open sectionC

	while @@FETCH_STATUS = 0
	begin
		set @sectionIndex = 1
		while @sectionIndex <= @sectionCount
		begin
			fetch absolute @sectionIndex from sectionC
			into @section
			insert into ScheduleSection(Id, Name, Number, MarkingPeriodRef)
			values(NEWID(), @section, @sectionIndex - 1, @mpId)
			set @sectionIndex = @sectionIndex + 1
		end
		fetch next from mpCursor
		into @mpId
	end
	CLOSE mpCursor;
	DEALLOCATE mpCursor;
	CLOSE sectionC;
	DEALLOCATE sectionC;

end
GO
