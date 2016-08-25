CREATE TABLE [dbo].[Document] (
    [Id]    UNIQUEIDENTIFIER NOT NULL,
    [Title] NVARCHAR (100)   NULL,
    CONSTRAINT [PK_Document] PRIMARY KEY CLUSTERED ([Id] ASC)
);

