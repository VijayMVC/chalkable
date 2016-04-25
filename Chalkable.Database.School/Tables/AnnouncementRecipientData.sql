CREATE TABLE [dbo].[AnnouncementRecipientData] (
    [AnnouncementRef] INT NOT NULL,
    [PersonRef]       INT NOT NULL,
    [Complete]        BIT NOT NULL,
    CONSTRAINT [PK_AdminAnnouncementData] PRIMARY KEY CLUSTERED ([AnnouncementRef] ASC, [PersonRef] ASC),
    CONSTRAINT [FK_AdminAnnouncementData_Announcement] FOREIGN KEY ([AnnouncementRef]) REFERENCES [dbo].[Announcement] ([Id]),
    CONSTRAINT [FK_AdminAnnouncementData_Person] FOREIGN KEY ([PersonRef]) REFERENCES [dbo].[Person] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_AnnouncementRecipientData_PersonRef
	ON dbo.AnnouncementRecipientData( PersonRef )
GO