alter procedure [dbo].[spGetSuggestedApplications] @academicBenchmarkIds nvarchar(max), @installedAppsIds nvarchar(max), @start int, @count int
as

declare @standardsIdsT table ([id] uniqueidentifier)
declare @installedAppsIdsT table ([id] uniqueidentifier)

if @academicBenchmarkIds is not null and RTRIM(LTRIM(@academicBenchmarkIds)) <> ''
begin
	insert into @standardsIdsT
	select distinct ABToCCMapping.CCStandardRef 
	from (select cast(s as uniqueidentifier) as [id] from dbo.split(',', @academicBenchmarkIds)) x
	join ABToCCMapping on ABToCCMapping.AcademicBenchmarkId = x.id
end
 
if @installedAppsIds is not null and RTRIM(LTRIM(@installedAppsIds)) <> ''
begin
	insert into @installedAppsIdsT
	select cast(s as uniqueidentifier) from dbo.split(',', @installedAppsIds)
end

declare @appT table (Id uniqueidentifier, name nvarchar(max), [rank] int)

insert into @appT
select * from (
select Application.Id, Application.Name, 
	(case when Application.InternalScore is null then 0 else Application.InternalScore end) 
	+ (count(StandardRef) * 100) 
	+ (count(appsIds.[Id]) * 100) as [Rank]

	from Application
	left join  ApplicationStandard on ApplicationStandard.ApplicationRef= Application.Id
	left join @standardsIdsT scT on scT.[id] = ApplicationStandard.StandardRef
	left join @installedAppsIdsT appsIds on appsIds.[id] = Application.Id
	where State = 5 and CanAttach = 1
	group by Application.Id, Application.Name, Application.InternalScore) app
where app.[Rank] > 0
order by app.[Rank] desc, app.[Name] asc
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY 

select Application.* from Application
join @appT as app on app.id = Application.id
order by app.[Rank] desc, app.Name asc
GO


