CREATE TABLE [dbo].[AnnouncementAssignedAttribute](
	[Id] [int] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Text] [nvarchar](MAX) NOT NULL,
	[AnnouncementRef] [int] NOT NULL,
	[Uuid] [nvarchar](255) NULL,
	[AttributeTypeId] [int] NOT NULL,
	[VisibleForStudents] [bit] NOT NULL
	)
GO

ALTER TABLE [dbo].[AnnouncementAssignedAttribute]  WITH NOCHECK ADD  CONSTRAINT [FK_AnnouncementAssignedAttribute_Announcement] FOREIGN KEY([AnnouncementRef])
REFERENCES [dbo].[Announcement] ([Id])
GO

ALTER TABLE [dbo].[AnnouncementAssignedAttribute] WITH NOCHECK ADD CONSTRAINT [FK_AnnouncementAssignedAttribute_AttributeTypeId] FOREIGN KEY([AttributeTypeId])
REFERENCES [dbo].[AnnouncementAttribute] ([Id])
GO

