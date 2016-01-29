CREATE TABLE [dbo].[ApplicationBanHistory] (
    [Id]             INT              IDENTITY (1, 1) NOT NULL,
    [ApplicationRef] UNIQUEIDENTIFIER NOT NULL,
    [PersonRef]      INT              NOT NULL,
    [Banned]         BIT              NOT NULL,
    [Date]           DATETIME2 (7)    NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApplicationBanHistory_Person] FOREIGN KEY ([PersonRef]) REFERENCES [dbo].[Person] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_ApplicationBanHistory_ApplicationRef]
    ON [dbo].[ApplicationBanHistory]([ApplicationRef] ASC);

