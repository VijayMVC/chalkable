Alter Table School
	Add StudyCenterEnabledTill DateTime2
GO
Drop type [dbo].[TSchool]
GO
Create Type [dbo].[TSchool] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[DistrictRef] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[LocalId] [int] NOT NULL,
	[IsChalkableEnabled] [bit] NOT NULL,
	StudyCenterEnabledTill DateTime2
)
GO