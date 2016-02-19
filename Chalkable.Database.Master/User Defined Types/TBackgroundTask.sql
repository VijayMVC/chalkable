CREATE TYPE [dbo].[TBackgroundTask] AS TABLE (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [DistrictRef] UNIQUEIDENTIFIER NULL,
    [Type]        INT              NOT NULL,
    [State]       INT              NOT NULL,
    [Created]     DATETIME2 (7)    NOT NULL,
    [Scheduled]   DATETIME2 (7)    NOT NULL,
    [Started]     DATETIME2 (7)    NULL,
    [Data]        NVARCHAR (MAX)   NULL,
    [Completed]   DATETIME2 (7)    NULL);

