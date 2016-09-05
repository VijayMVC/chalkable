CREATE TABLE [dbo].[ApplicationStandard] (
    [ApplicationRef] UNIQUEIDENTIFIER NOT NULL,
    [StandardRef]    UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_ApplicationStandardID] PRIMARY KEY CLUSTERED ([ApplicationRef] ASC, [StandardRef] ASC),
    CONSTRAINT [FK_ApplicationStandard_Application] FOREIGN KEY ([ApplicationRef]) REFERENCES [dbo].[Application] ([Id])
);

