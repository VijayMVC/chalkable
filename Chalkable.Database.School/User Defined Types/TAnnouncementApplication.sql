CREATE TYPE [dbo].[TAnnouncementApplication] AS TABLE (
    [Id]              INT              NOT NULL,
    [AnnouncementRef] INT              NOT NULL,
    [ApplicationRef]  UNIQUEIDENTIFIER NOT NULL,
    [Active]          BIT              NOT NULL,
    [Order]           INT              NOT NULL);

