CREATE TABLE [dbo].[AnnouncementApplication] (
    [Id]				INT					IDENTITY (1, 1) NOT NULL,
    [AnnouncementRef]	INT					NOT NULL,
    [ApplicationRef]	UNIQUEIDENTIFIER	NOT NULL,
    [Active]			BIT					NOT NULL,
    [Order]				INT					NOT NULL,
	[Text]				NVARCHAR(100)		NULL,
	[ImageUrl]			NVARCHAR(MAX)		NULL,
	[Description]		NVARCHAR(256)		NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AnnouncementApplication_Announcement] FOREIGN KEY ([AnnouncementRef]) REFERENCES [dbo].[Announcement] ([Id])
);

GO

CREATE NONCLUSTERED INDEX IX_AnnouncementApplication_AnnouncementRef
	ON dbo.AnnouncementApplication( AnnouncementRef )
GO