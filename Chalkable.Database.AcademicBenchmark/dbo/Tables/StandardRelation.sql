CREATE TABLE [dbo].[StandardRelation] (
    [StandardRef]   UNIQUEIDENTIFIER NOT NULL,
    [DerivativeRef] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_StandardRelation] PRIMARY KEY CLUSTERED ([StandardRef] ASC, [DerivativeRef] ASC),
    CONSTRAINT [FK_StandardRelation_Standard] FOREIGN KEY ([StandardRef]) REFERENCES [dbo].[Standard] ([Id]),
    CONSTRAINT [FK_StandardRelation_Standard_Derivative] FOREIGN KEY ([DerivativeRef]) REFERENCES [dbo].[Standard] ([Id])
);



