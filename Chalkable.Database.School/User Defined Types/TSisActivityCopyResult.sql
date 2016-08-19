CREATE TYPE [dbo].[TSisActivityCopyResult] AS TABLE
(
	[FromActivityId] int,
	[ToActivityId] int,
	[ToClassId] int
)
