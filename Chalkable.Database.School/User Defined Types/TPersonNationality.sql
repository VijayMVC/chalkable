CREATE TYPE [dbo].[TPersonNationality] AS TABLE
(
	[Id] INT, 
    [PersonRef] INT, 
    [Nationality] NVARCHAR(128), 
    [IsPrimary] BIT, 
    [CountryRef] INT
)
