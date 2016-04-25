CREATE TABLE [dbo].[AutoGrade] (
    [AnnouncementApplicationRef] INT            NOT NULL,
    [StudentRef]                 INT            NOT NULL,
    [Grade]                      NVARCHAR (255) NULL,
    [Date]                       DATETIME2 (7)  NULL,
    [Posted]                     BIT            NULL,
    CONSTRAINT [PK_AutoGrade] PRIMARY KEY CLUSTERED ([StudentRef] ASC, [AnnouncementApplicationRef] ASC),
    CONSTRAINT [FK_AutoGrade_AnnouncementApplication] FOREIGN KEY ([AnnouncementApplicationRef]) REFERENCES [dbo].[AnnouncementApplication] ([Id]),
    CONSTRAINT [FK_AutoGrade_Student] FOREIGN KEY ([StudentRef]) REFERENCES [dbo].[Student] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_AutoGrade_AnnouncementApplicationRef
	ON dbo.AutoGrade( AnnouncementApplicationRef )
GO