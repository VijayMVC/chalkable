CREATE TABLE [dbo].[AttendanceMonth] (
    [Id]                 INT           NOT NULL,
    [SchoolYearRef]      INT           NOT NULL,
    [Name]               NVARCHAR (30) NOT NULL,
    [StartDate]          DATETIME2 (7) NOT NULL,
    [EndDate]            DATETIME2 (7) NOT NULL,
    [EndTime]            DATETIME2 (7) NOT NULL,
    [IsLockedAttendance] BIT           NOT NULL,
    [IsLockedDiscipline] BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AttendanceMonth_SchoolYear] FOREIGN KEY ([SchoolYearRef]) REFERENCES [dbo].[SchoolYear] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_AttendanceMonth_SchoolYearRef
	ON dbo.AttendanceMonth( SchoolYearRef )
GO