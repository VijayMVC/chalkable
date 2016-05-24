CREATE TYPE [dbo].[TPeriod] AS TABLE (
    [Id]            INT           NOT NULL,
    [SchoolYearRef] INT           NOT NULL,
    [Order]         INT           NOT NULL,
    [Name]          NVARCHAR (20) NOT NULL);

