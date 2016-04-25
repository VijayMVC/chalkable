CREATE TABLE [dbo].[Notification] (
    [Id]                INT              IDENTITY (1, 1) NOT NULL,
    [Type]              INT              NOT NULL,
    [Message]           NVARCHAR (1024)  NULL,
    [Shown]             BIT              NOT NULL,
    [PersonRef]         INT              NOT NULL,
    [AnnouncementRef]   INT              NULL,
    [PrivateMessageRef] INT              NULL,
    [ApplicationRef]    UNIQUEIDENTIFIER NULL,
    [QuestionPersonRef] INT              NULL,
    [Created]           DATETIME2 (7)    NOT NULL,
    [MarkingPeriodRef]  INT              NULL,
    [WasSend]           BIT              NOT NULL,
    [RoleRef]           INT              NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Notification_Announcement] FOREIGN KEY ([AnnouncementRef]) REFERENCES [dbo].[Announcement] ([Id]),
    CONSTRAINT [FK_Notification_MarkingPeriod] FOREIGN KEY ([MarkingPeriodRef]) REFERENCES [dbo].[MarkingPeriod] ([Id]),
    CONSTRAINT [FK_Notification_Person] FOREIGN KEY ([PersonRef]) REFERENCES [dbo].[Person] ([Id]),
    CONSTRAINT [FK_Notification_PrivateMessage] FOREIGN KEY ([PrivateMessageRef]) REFERENCES [dbo].[PrivateMessage] ([Id]),
    CONSTRAINT [FK_Notification_QuestionPerson] FOREIGN KEY ([QuestionPersonRef]) REFERENCES [dbo].[Person] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_Notification_AnnouncementRef
	ON dbo.Notification( AnnouncementRef )
GO


CREATE NONCLUSTERED INDEX IX_Notification_MarkingPeriodRef
	ON dbo.Notification( MarkingPeriodRef )
GO


CREATE NONCLUSTERED INDEX IX_Notification_PersonRef
	ON dbo.Notification( PersonRef )
GO

CREATE NONCLUSTERED INDEX IX_Notification_PrivateMessageRef
	ON dbo.Notification( PrivateMessageRef )
GO

CREATE NONCLUSTERED INDEX IX_Notification_QuestionPersonRef
	ON dbo.Notification( QuestionPersonRef )
GO

