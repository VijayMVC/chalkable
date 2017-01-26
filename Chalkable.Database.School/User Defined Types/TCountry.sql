CREATE TYPE [dbo].[TCountry] AS TABLE
(
	[Id] INT, 
    [Code] NVARCHAR(64), 
    [Name] NVARCHAR(64), 
    [Description] NVARCHAR(512), 
    [StateCode] NVARCHAR(64), 
    [SIFCode] NVARCHAR(64), 
    [NCESCode] NVARCHAR(64), 
    [IsActive] BIT, 
    [IsSystem] BIT
)
