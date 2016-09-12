CREATE TABLE [dbo].[SubjectDoc] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [Description] NVARCHAR (256)   NULL,
    CONSTRAINT [PK_SubjectDoc] PRIMARY KEY CLUSTERED ([Id] ASC)
);

