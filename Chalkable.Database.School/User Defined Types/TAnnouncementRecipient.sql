CREATE TYPE [dbo].[TAnnouncementRecipient] AS TABLE (
    [Id]              INT NOT NULL,
    [AnnouncementRef] INT NOT NULL,
    [ToAll]           BIT NOT NULL,
    [RoleRef]         INT NULL,
    [GradeLevelRef]   INT NULL,
    [PersonRef]       INT NULL);

