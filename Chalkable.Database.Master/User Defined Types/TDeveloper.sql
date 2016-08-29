CREATE TYPE [dbo].[TDeveloper] AS TABLE (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (255)   NULL,
    [WebSite]     NVARCHAR (255)   NULL,
    [DistrictRef] UNIQUEIDENTIFIER NOT NULL);

