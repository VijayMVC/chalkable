CREATE TABLE [dbo].[ApplicationPermission] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [ApplicationRef] UNIQUEIDENTIFIER NOT NULL,
    [Permission]     INT              NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApplicationPermission_Application] FOREIGN KEY ([ApplicationRef]) REFERENCES [dbo].[Application] ([Id]),
    CONSTRAINT [UQ_ApplicationPermission_ApplicationRef_Permission] UNIQUE NONCLUSTERED ([ApplicationRef] ASC, [Permission] ASC)
);

