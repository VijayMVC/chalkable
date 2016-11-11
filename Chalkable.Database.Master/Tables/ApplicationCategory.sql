CREATE TABLE [dbo].[ApplicationCategory] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [ApplicationRef] UNIQUEIDENTIFIER NOT NULL,
    [CategoryRef]    UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApplicationCategory_Application] FOREIGN KEY ([ApplicationRef]) REFERENCES [dbo].[Application] ([Id]),
    CONSTRAINT [FK_ApplicationCategory_Category] FOREIGN KEY ([CategoryRef]) REFERENCES [dbo].[Category] ([Id])
);

