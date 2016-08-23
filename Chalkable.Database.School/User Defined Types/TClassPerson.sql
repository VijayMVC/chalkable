CREATE TYPE [dbo].[TClassPerson] AS TABLE (
    [ClassRef]         INT NOT NULL,
    [PersonRef]        INT NOT NULL,
    [MarkingPeriodRef] INT NOT NULL,
    [IsEnrolled]       BIT NOT NULL);

