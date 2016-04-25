CREATE TABLE [dbo].[AnnouncementStandard] (
    [StandardRef]     INT NOT NULL,
    [AnnouncementRef] INT NOT NULL,
    CONSTRAINT [PK_AnnouncementStandard] PRIMARY KEY CLUSTERED ([StandardRef] ASC, [AnnouncementRef] ASC),
    CONSTRAINT [FK_AnnouncementStandard_Announcement] FOREIGN KEY ([AnnouncementRef]) REFERENCES [dbo].[Announcement] ([Id]),
    CONSTRAINT [FK_AnnouncementStandard_Standard] FOREIGN KEY ([StandardRef]) REFERENCES [dbo].[Standard] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_AnnouncementStandard_AnnouncementRef
	ON dbo.AnnouncementStandard( AnnouncementRef )
GO
