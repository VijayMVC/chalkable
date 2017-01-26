CREATE TABLE [dbo].[Course] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [Description] NVARCHAR (256)   NULL,
    CONSTRAINT [PK_Course] PRIMARY KEY CLUSTERED ([Id] ASC)
);

