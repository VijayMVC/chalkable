CREATE TYPE [dbo].[TStaff] AS TABLE (
    [Id]        INT            NOT NULL,
    [FirstName] NVARCHAR (255) NOT NULL,
    [LastName]  NVARCHAR (255) NOT NULL,
    [BirthDate] DATETIME2 (7)  NULL,
    [Gender]    NVARCHAR (255) NULL,
    [UserId]    INT            NULL);

