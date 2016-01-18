CREATE TYPE [dbo].[TApplicationCategory] AS TABLE (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [ApplicationRef] UNIQUEIDENTIFIER NOT NULL,
    [CategoryRef]    UNIQUEIDENTIFIER NOT NULL);

