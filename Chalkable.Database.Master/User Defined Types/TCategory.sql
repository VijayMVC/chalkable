CREATE TYPE [dbo].[TCategory] AS TABLE (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (255)   NOT NULL,
    [Description] NVARCHAR (1024)  NULL);

