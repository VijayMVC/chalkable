﻿CREATE TYPE [dbo].[TStudentSchool] AS TABLE (
    [StudentRef] INT NOT NULL,
    [SchoolRef]  INT NOT NULL,
	[CounselorRef] INT NULL,
	[IsTitle1Eligible] BIT NOT NULL
);

