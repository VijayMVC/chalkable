CREATE TABLE [dbo].[Announcement] (
    [Id]      INT            IDENTITY (1000000000, 1) NOT NULL,
    [Content] NVARCHAR (MAX) NULL,
    [Created] DATETIME2 (7)  NOT NULL,
    [State]   INT            NOT NULL,
    [Title]   NVARCHAR (101)  NULL,
	[DiscussionEnabled] BIT NOT NULL DEFAULT 0,
	[PreviewCommentsEnabled] BIT NOT NULL DEFAULT 0,
	[RequireCommentsEnabled] BIT NOT NULL DEFAULT 0,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

