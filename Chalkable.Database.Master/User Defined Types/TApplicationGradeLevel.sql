CREATE TYPE [dbo].[TApplicationGradeLevel] AS TABLE (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [ApplicationRef] UNIQUEIDENTIFIER NOT NULL,
    [GradeLevel]     INT              NOT NULL);

