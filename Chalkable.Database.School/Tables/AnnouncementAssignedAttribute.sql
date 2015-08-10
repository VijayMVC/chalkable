CREATE TABLE [dbo].[AnnouncementAssignedAttribute] (
    [Id]                             INT            IDENTITY (1, 1) NOT NULL,
    [Name]                           NVARCHAR (255) NOT NULL,
    [Text]                           NVARCHAR (MAX) NOT NULL,
    [AnnouncementRef]                INT            NOT NULL,
    [Uuid]                           NVARCHAR (255) NULL,
    [AttributeTypeId]                INT            NOT NULL,
    [VisibleForStudents]             BIT            NOT NULL,
    [SisActivityAssignedAttributeId] INT            NULL,
    [SisAttributeAttachmentId]       INT            NULL,
    [SisAttachmentName]              NVARCHAR (255) NULL,
    [SisAttachmentMimeType]          NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AnnouncementAssignedAttribute_Announcement] FOREIGN KEY ([AnnouncementRef]) REFERENCES [dbo].[Announcement] ([Id]),
    CONSTRAINT [FK_AnnouncementAssignedAttribute_AttributeTypeId] FOREIGN KEY ([AttributeTypeId]) REFERENCES [dbo].[AnnouncementAttribute] ([Id])
);

