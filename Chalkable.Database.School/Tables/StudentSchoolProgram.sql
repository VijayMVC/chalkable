CREATE TABLE [dbo].[StudentSchoolProgram]
(
    [StudentSchoolProgramID] INT IDENTITY (1,1) NOT NULL CONSTRAINT [NCPK_StudentSchoolProgram] PRIMARY KEY NONCLUSTERED ( [StudentSchoolProgramID] ),
    [StudentID] INT NOT NULL,
    [AcadSessionID] INT NOT NULL,
    [SchoolProgramID] SMALLINT NOT NULL,
    [StartDate] DATETIME NOT NULL,
    [EndDate] DATETIME NULL,
    [EndTime] DATETIME NULL,
    [DecimalValue] DECIMAL(5, 2) NULL,
    [RowVersion] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_StudentSchoolProgram_RowVersion] DEFAULT (NEWID()),
	[DistrictGuid] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [UCC_StudentSchoolProgram_StudentID_AcadSessionID_SchoolProgramID_StartDate] UNIQUE CLUSTERED ( [StudentID], [AcadSessionID], [SchoolProgramID],[StartDate] )

)
