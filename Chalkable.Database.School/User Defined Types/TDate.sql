CREATE TYPE [dbo].[TDate] AS TABLE (
    [Day]             DATETIME2 (7) NOT NULL,
    [DayTypeRef]      INT           NULL,
    [SchoolYearRef]   INT           NOT NULL,
    [IsSchoolDay]     BIT           NOT NULL,
    [BellScheduleRef] INT           NULL);

