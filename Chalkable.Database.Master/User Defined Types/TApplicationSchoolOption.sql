CREATE TYPE [dbo].[TApplicationSchoolOption] AS TABLE
(
	ApplicationRef uniqueidentifier, 
	SchoolRef uniqueidentifier,
	Banned bit
)
