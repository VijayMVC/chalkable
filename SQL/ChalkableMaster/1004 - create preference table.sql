CREATE TABLE [dbo].[Preference](
	[Id] uniqueidentifier primary key NOT NULL,
	[Key] [nvarchar](256) NOT NULL,
	[Value] [nvarchar](2046) NULL,
	[IsPublic] [bit] NOT NULL,
	[Category] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[Hint] [nvarchar](max) NULL
)
go

CREATE TABLE [dbo].[ImportSystemType](
	[Type] [int] primary key NOT NULL,
	[Name] [nvarchar](255) NOT NULL
)
go
