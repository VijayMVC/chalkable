CREATE TYPE [dbo].[TPersonEmail] AS TABLE (
    [PersonRef]    INT            NOT NULL,
    [EmailAddress] NVARCHAR (128) NOT NULL,
    [Description]  NVARCHAR (MAX) NOT NULL,
    [IsListed]     BIT            NULL,
    [IsPrimary]    BIT            NULL);

