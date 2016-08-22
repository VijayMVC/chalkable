CREATE TABLE [dbo].[ApplicationPicture] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [ApplicationRef] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_ApplicationPicture_Id_ApplicationRef] PRIMARY KEY CLUSTERED ([Id] ASC, [ApplicationRef] ASC),
    CONSTRAINT [FK_ApplicationPicture_Application] FOREIGN KEY ([ApplicationRef]) REFERENCES [dbo].[Application] ([Id])
);

