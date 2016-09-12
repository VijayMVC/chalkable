CREATE TYPE [dbo].[TStudentSchoolProgram] AS TABLE
(
	[StudentSchoolProgramID] INT NOT NULL,
    [StudentID] INT NOT NULL,
    [AcadSessionID] INT NOT NULL,
    [SchoolProgramID] SMALLINT NOT NULL,
    [StartDate] DATETIME NOT NULL,
    [EndDate] DATETIME NULL,
    [EndTime] DATETIME NULL,
    [DecimalValue] DECIMAL(5, 2) NULL,
    [RowVersion] UNIQUEIDENTIFIER NOT NULL,
	[DistrictGuid] UNIQUEIDENTIFIER NOT NULL
)
