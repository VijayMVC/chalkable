CREATE TABLE [dbo].[PersonEmail] (
    [PersonRef]    INT            NOT NULL,
    [EmailAddress] NVARCHAR (128) NOT NULL,
    [Description]  NVARCHAR (MAX) NOT NULL,
    [IsListed]     BIT            NULL,
    [IsPrimary]    BIT            NULL,
    CONSTRAINT [PK_PersonRef_EmailAddress] PRIMARY KEY CLUSTERED ([PersonRef] ASC, [EmailAddress] ASC),
    CONSTRAINT [FK_PersonEmail_Person] FOREIGN KEY ([PersonRef]) REFERENCES [dbo].[Person] ([Id])
);

