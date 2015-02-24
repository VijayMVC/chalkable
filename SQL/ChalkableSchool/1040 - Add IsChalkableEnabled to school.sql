Alter Table School
	Add IsChalkableEnabled bit
GO
Update School Set IsChalkableEnabled = 1
GO
Alter Table School 
	Alter Column IsChalkableEnabled bit not null
GO
DROP TYPE [TSchool]

CREATE TYPE [dbo].[TSchool] AS TABLE (
	[Id] [int] NOT NULL,
	[Name] [nvarchar](1024) NULL,
	[IsActive] [bit] NOT NULL,
	[IsPrivate] [bit] NOT NULL,
	IsChalkableEnabled bit NOT NULL
)