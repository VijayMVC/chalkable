CREATE TABLE [dbo].[Person] (
    [Id]                   INT            NOT NULL,
    [FirstName]            NVARCHAR (255) NOT NULL,
    [LastName]             NVARCHAR (255) NOT NULL,
    [BirthDate]            DATETIME2 (7)  NULL,
    [Gender]               NVARCHAR (255) NULL,
    [Salutation]           NVARCHAR (255) NULL,
    [Active]               BIT            NOT NULL,
    [FirstLoginDate]       DATETIME2 (7)  NULL,
    [LastMailNotification] DATETIME2 (7)  NULL,
    [AddressRef]           INT            NULL,
    [PhotoModifiedDate]    DATETIME2 (7)  NULL,
    [UserId]               INT            NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Person_Address] FOREIGN KEY ([AddressRef]) REFERENCES [dbo].[Address] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_Person_AddressRef
	ON dbo.Person( AddressRef )
GO
