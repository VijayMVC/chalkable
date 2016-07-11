CREATE TYPE [dbo].[TDayType] AS TABLE (
    [Id]            INT             NOT NULL,
    [Number]        INT             NOT NULL,
    [Name]          NVARCHAR (1024) NOT NULL,
    [SchoolYearRef] INT             NOT NULL);

