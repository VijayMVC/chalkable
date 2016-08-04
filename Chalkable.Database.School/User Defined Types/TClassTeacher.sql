CREATE TYPE [dbo].[TClassTeacher] AS TABLE (
    [PersonRef]         INT NOT NULL,
    [ClassRef]          INT NOT NULL,
    [IsHighlyQualified] BIT NOT NULL,
    [IsCertified]       BIT NOT NULL,
    [IsPrimary]         BIT NOT NULL);

