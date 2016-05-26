CREATE procedure [dbo].[spGetSuggestedApplications] 
	@academicBenchmarkIds TGUID ReadOnly,
	@start int,
	@count int
as

declare @appT table 
(
	Id uniqueidentifier, 
	name nvarchar(max), 
	[rank] int
)

insert into @appT
select * 
from (
	select 
		[Application].Id, 
		[Application].Name,
		(case when [Application].InternalScore is null then 0 else [Application].InternalScore end)
			+ (count(StandardRef) * 100) as [Rank]
	from 
	[Application]
	join  ApplicationStandard 
		on ApplicationStandard.ApplicationRef= Application.Id
	join @academicBenchmarkIds abT
		on abT.Value = ApplicationStandard.StandardRef
	Where 
		[State] = 5 
		and IsInternal = 0
	group by 
		Application.Id, 
		Application.Name, 
		Application.InternalScore) app
where 
	app.[Rank] > 0
order by app.[Rank] desc, app.[Name] asc
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY

select 
	Application.* 
from Application
	join @appT as app on app.id = Application.Id
order by app.[Rank] desc, app.Name asc
