Create Type [dbo].[TAnnouncementAssignedAttribute] As Table(
	[Id]                             INT            NOT NULL,
    [AnnouncementRef]                INT            NOT NULL,
	[AttributeTypeId]                INT            NOT NULL,
	[AttachmentRef]                  INT            NULL,
    [Name]                           NVARCHAR (255) NOT NULL,
    [Text]                           NVARCHAR (MAX) NOT NULL,
    [VisibleForStudents]             BIT            NOT NULL,
    [SisActivityAssignedAttributeId] INT            NULL,
    [SisActivityId]                  INT            NULL
)