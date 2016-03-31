CREATE TYPE [dbo].[TSisUser] AS TABLE (
    [Id]         INT            NOT NULL,
    [UserName]   NVARCHAR (127) NOT NULL,
    [LockedOut]  BIT            NOT NULL,
    [IsDisabled] BIT            NOT NULL,
    [IsSystem]   BIT            NOT NULL);

