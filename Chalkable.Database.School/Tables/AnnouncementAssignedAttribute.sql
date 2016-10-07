CREATE TABLE [dbo].[AnnouncementAssignedAttribute] (
    [Id]                             INT            IDENTITY (1, 1) NOT NULL,
    [Name]                           NVARCHAR (255) NOT NULL,
    [Text]                           NVARCHAR (MAX) NOT NULL,
    [AnnouncementRef]                INT            NOT NULL,
    [AttributeTypeId]                INT            NOT NULL,
    [VisibleForStudents]             BIT            NOT NULL,
    [SisActivityAssignedAttributeId] INT            NULL,
    [AttachmentRef]                  INT            NULL,
    [SisActivityId]                  INT            NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AnnouncementAssignedAttribute_Announcement] FOREIGN KEY ([AnnouncementRef]) REFERENCES [dbo].[Announcement] ([Id]),
    CONSTRAINT [FK_AnnouncementAssignedAttribute_Attachment] FOREIGN KEY ([AttachmentRef]) REFERENCES [dbo].[Attachment] ([Id]),
    CONSTRAINT [FK_AnnouncementAssignedAttribute_AttributeTypeId] FOREIGN KEY ([AttributeTypeId]) REFERENCES [dbo].[AnnouncementAttribute] ([Id])
);


GO

CREATE NONCLUSTERED INDEX IX_AnnouncementAssignedAttribute_AnnouncementRef
	ON dbo.AnnouncementAssignedAttribute( AnnouncementRef )
GO


CREATE NONCLUSTERED INDEX IX_AnnouncementAssignedAttribute_AttachmentRef
	ON dbo.AnnouncementAssignedAttribute( AttachmentRef )
GO


CREATE NONCLUSTERED INDEX IX_AnnouncementAssignedAttribute_AttributeTypeId
	ON dbo.AnnouncementAssignedAttribute( AttributeTypeId )
GO