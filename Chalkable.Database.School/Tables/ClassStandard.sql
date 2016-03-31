CREATE TABLE [dbo].[ClassStandard] (
    [ClassRef]    INT NOT NULL,
    [StandardRef] INT NOT NULL,
    CONSTRAINT [PK_ClassStandard] PRIMARY KEY CLUSTERED ([ClassRef] ASC, [StandardRef] ASC),
    CONSTRAINT [FK_ClassStandard_Class] FOREIGN KEY ([ClassRef]) REFERENCES [dbo].[Class] ([Id]),
    CONSTRAINT [FK_ClassStandard_Standard] FOREIGN KEY ([StandardRef]) REFERENCES [dbo].[Standard] ([Id])
);

