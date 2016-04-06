Alter Table School
	Add IsChalkableEnabled bit
GO
Update School Set IsChalkableEnabled = 1
GO
Alter Table School 
	Alter Column IsChalkableEnabled bit not null
GO
DROP TYPE [TSchool]
CREATE TYPE [TSchool] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[DistrictRef] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[LocalId] [int] NOT NULL,
	IsChalkableEnabled bit not null
) 