CREATE TABLE [dbo].[Announcement] (
    [Id]      INT            IDENTITY (1000000000, 1) NOT NULL,
    [Content] NVARCHAR (MAX) NULL,
    [Created] DATETIME2 (7)  NOT NULL,
    [State]   INT            NOT NULL,
    [Title]   NVARCHAR (101)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

