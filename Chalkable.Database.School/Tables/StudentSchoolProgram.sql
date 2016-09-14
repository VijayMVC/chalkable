CREATE TABLE [dbo].[StudentSchoolProgram]
(
    [Id] INT IDENTITY (1,1) NOT NULL CONSTRAINT [NCPK_StudentSchoolProgram] PRIMARY KEY NONCLUSTERED ( [Id] ),
    [StudentId] INT NOT NULL,
    [AcadSessionId] INT NOT NULL,
    [SchoolProgramId] INT NOT NULL,
    [StartDate] DATETIME NOT NULL,
    [EndDate] DATETIME NULL,
    [EndTime] DATETIME NULL,
    [DecimalValue] DECIMAL(5, 2) NULL,
    CONSTRAINT [UCC_StudentSchoolProgram_StudentID_AcadSessionID_SchoolProgramID_StartDate] UNIQUE CLUSTERED ( [StudentID], [AcadSessionID], [SchoolProgramID],[StartDate] )
)
