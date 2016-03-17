CREATE TYPE [dbo].[TStandardSubject] AS TABLE (
    [Id]           INT            NOT NULL,
    [Name]         NVARCHAR (100) NOT NULL,
    [Description]  NVARCHAR (200) NOT NULL,
    [AdoptionYear] INT            NULL,
    [IsActive]     BIT            NOT NULL);

