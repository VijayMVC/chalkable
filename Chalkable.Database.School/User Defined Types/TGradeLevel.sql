CREATE TYPE [dbo].[TGradeLevel] AS TABLE (
    [Id]          INT             NOT NULL,
    [Name]        NVARCHAR (255)  NOT NULL,
    [Description] NVARCHAR (1024) NULL,
    [Number]      INT             NOT NULL);

