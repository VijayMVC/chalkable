

drop TYPE [TNotification]
go
CREATE TYPE [dbo].[TNotification] AS TABLE(

	[Type] [int] NOT NULL,
	[Message] [nvarchar](1024) NULL,
	[Shown] [bit] NOT NULL,
	[PersonRef] [int] NOT NULL,
	[AnnouncementRef] [int] NULL,
	[PrivateMessageRef] [int] NULL,
	[ApplicationRef] [uniqueidentifier] NULL,
	[QuestionPersonRef] [int] NULL,
	[Created] [datetime2](7) NOT NULL,
	[MarkingPeriodRef] [int] NULL,
	[WasSend] [bit] NOT NULL
)
GO
