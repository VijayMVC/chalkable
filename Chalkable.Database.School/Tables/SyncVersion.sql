CREATE TABLE [dbo].[SyncVersion] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [TableName] NVARCHAR (256) NULL,
    [Version]   BIGINT         NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

