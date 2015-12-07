CREATE TYPE [dbo].[TBellSchedule] AS TABLE (
    [Id]              INT             NOT NULL,
    [SchoolYearRef]   INT             NOT NULL,
    [Name]            NVARCHAR (255)  NULL,
    [Description]     NVARCHAR (1024) NULL,
    [TotalMinutes]    INT             NOT NULL,
    [Code]            NVARCHAR (255)  NULL,
    [IsActive]        BIT             NOT NULL,
    [IsSystem]        BIT             NOT NULL,
    [UseStartEndTime] BIT             NOT NULL);

