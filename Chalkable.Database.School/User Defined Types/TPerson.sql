CREATE TYPE [dbo].[TPerson] AS TABLE (
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
	[IsHispanic]		   BIT
);

