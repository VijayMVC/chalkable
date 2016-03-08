CREATE TABLE [dbo].[CommonCoreStandard] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [ParentStandardRef]   UNIQUEIDENTIFIER NULL,
    [StandardCategoryRef] UNIQUEIDENTIFIER NOT NULL,
    [Code]                NVARCHAR (255)   NOT NULL,
    [Description]         NVARCHAR (MAX)   NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CommonCoreStandard_ParentStandard] FOREIGN KEY ([ParentStandardRef]) REFERENCES [dbo].[CommonCoreStandard] ([Id]),
    CONSTRAINT [FK_CommonCoreStandard_StandardCategory] FOREIGN KEY ([StandardCategoryRef]) REFERENCES [dbo].[CommonCoreStandardCategory] ([Id])
);

