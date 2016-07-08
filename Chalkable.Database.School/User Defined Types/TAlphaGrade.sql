CREATE TYPE [dbo].[TAlphaGrade] AS TABLE (
    [Id]          INT            NOT NULL,
    [SchoolRef]   INT            NOT NULL,
    [Name]        NVARCHAR (5)   NOT NULL,
    [Description] NVARCHAR (255) NOT NULL);

