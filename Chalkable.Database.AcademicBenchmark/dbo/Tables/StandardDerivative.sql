CREATE TABLE [dbo].[StandardDerivative] (
    [StandardRef]   UNIQUEIDENTIFIER NOT NULL,
    [DerivativeRef] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_StandardDerivative] PRIMARY KEY CLUSTERED ([StandardRef] ASC, [DerivativeRef] ASC)
);



