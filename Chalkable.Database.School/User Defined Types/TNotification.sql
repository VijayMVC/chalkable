CREATE TYPE [dbo].[TNotification] AS TABLE (
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
    [WasSend]           BIT              NOT NULL);

