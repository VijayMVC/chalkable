CREATE TABLE [dbo].[Category] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (255)   NOT NULL,
    [Description] NVARCHAR (1024)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

