CREATE TYPE [dbo].[TStudentSchoolProgram] AS TABLE
(
	[Id] INT NOT NULL,
    [StudentId] INT NOT NULL,
    [AcadSessionId] INT NOT NULL,
    [SchoolProgramId] INT NOT NULL,
    [StartDate] DATETIME NOT NULL,
    [EndDate] DATETIME NULL,
    [EndTime] DATETIME NULL,
    [DecimalValue] DECIMAL(5, 2) NULL
)
