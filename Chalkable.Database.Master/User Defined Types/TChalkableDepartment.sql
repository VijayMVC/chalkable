CREATE TYPE [dbo].[TChalkableDepartment] AS TABLE (
    [Id]       UNIQUEIDENTIFIER NOT NULL,
    [Name]     NVARCHAR (255)   NOT NULL,
    [Keywords] NVARCHAR (MAX)   NOT NULL);

