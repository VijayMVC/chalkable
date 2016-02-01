CREATE TYPE [dbo].[TApplicationRating] AS TABLE (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [ApplicationRef] UNIQUEIDENTIFIER NOT NULL,
    [UserRef]        UNIQUEIDENTIFIER NOT NULL,
    [Rating]         INT              NOT NULL,
    [Review]         NVARCHAR (MAX)   NULL);

