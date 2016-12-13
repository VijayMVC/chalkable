CREATE TABLE [dbo].[Preference] (
    [Id]       UNIQUEIDENTIFIER NOT NULL,
    [Key]      NVARCHAR (256)   NOT NULL,
    [Value]    NVARCHAR (2046)  NULL,
    [IsPublic] BIT              NOT NULL,
    [Category] INT              NOT NULL,
    [Type]     INT              NOT NULL,
    [Hint]     NVARCHAR (MAX)   NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_KEY] UNIQUE NONCLUSTERED ([Key] ASC)
);

