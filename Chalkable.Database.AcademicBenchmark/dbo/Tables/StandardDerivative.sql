CREATE TABLE [dbo].[StandardDerivative] (
    [StandardRef]   UNIQUEIDENTIFIER NOT NULL,
    [DerivativeRef] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_StandardDerivative] PRIMARY KEY CLUSTERED ([StandardRef] ASC, [DerivativeRef] ASC),
    CONSTRAINT [FK_StandardDerivative_Standard] FOREIGN KEY ([StandardRef]) REFERENCES [dbo].[Standard] ([Id]),
    CONSTRAINT [FK_StandardDerivative_Standard_Derivative] FOREIGN KEY ([DerivativeRef]) REFERENCES [dbo].[Standard] ([Id])
);





