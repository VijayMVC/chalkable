CREATE TABLE [dbo].[Phone] (
    [PersonRef]      INT            NOT NULL,
    [Value]          NVARCHAR (256) NOT NULL,
    [Type]           INT            NOT NULL,
    [IsPrimary]      BIT            NOT NULL,
    [DigitOnlyValue] NVARCHAR (256) NOT NULL,
    CONSTRAINT [PK_Phone] PRIMARY KEY CLUSTERED ([DigitOnlyValue] ASC, [PersonRef] ASC),
    CONSTRAINT [FK_Phone_Person] FOREIGN KEY ([PersonRef]) REFERENCES [dbo].[Person] ([Id])
);

