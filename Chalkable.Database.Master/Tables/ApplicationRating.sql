CREATE TABLE [dbo].[ApplicationRating] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [ApplicationRef] UNIQUEIDENTIFIER NOT NULL,
    [UserRef]        UNIQUEIDENTIFIER NOT NULL,
    [Rating]         INT              NOT NULL,
    [Review]         NVARCHAR (MAX)   NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApplicationRating_Application] FOREIGN KEY ([ApplicationRef]) REFERENCES [dbo].[Application] ([Id]),
    CONSTRAINT [FK_ApplicationRating_User] FOREIGN KEY ([UserRef]) REFERENCES [dbo].[User] ([Id])
);



