CREATE TYPE [dbo].[TAnnouncementAttachment] AS TABLE (
    [Id]              INT            NOT NULL,
    [Name]            NVARCHAR (255) NOT NULL,
    [PersonRef]       INT            NOT NULL,
    [AnnouncementRef] INT            NOT NULL,
    [AttachedDate]    DATETIME2 (7)  NOT NULL,
    [Uuid]            NVARCHAR (255) NULL,
    [Order]           INT            NOT NULL,
    [SisAttachmentId] INT            NULL);

