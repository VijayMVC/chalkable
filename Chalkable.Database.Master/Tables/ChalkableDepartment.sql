CREATE TABLE [dbo].[ChalkableDepartment] (
    [Id]       UNIQUEIDENTIFIER NOT NULL,
    [Name]     NVARCHAR (255)   NOT NULL,
    [Keywords] NVARCHAR (MAX)   NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

