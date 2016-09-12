CREATE TABLE [dbo].[Authority] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [Code]        NVARCHAR (10)    NULL,
    [Description] NVARCHAR (256)   NULL,
    CONSTRAINT [PK_Authority] PRIMARY KEY CLUSTERED ([Id] ASC)
);

