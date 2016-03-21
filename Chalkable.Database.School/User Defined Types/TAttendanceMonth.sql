CREATE TYPE [dbo].[TAttendanceMonth] AS TABLE (
    [Id]                 INT           NOT NULL,
    [SchoolYearRef]      INT           NOT NULL,
    [Name]               NVARCHAR (30) NOT NULL,
    [StartDate]          DATETIME2 (7) NOT NULL,
    [EndDate]            DATETIME2 (7) NOT NULL,
    [EndTime]            DATETIME2 (7) NOT NULL,
    [IsLockedAttendance] BIT           NOT NULL,
    [IsLockedDiscipline] BIT           NOT NULL);

