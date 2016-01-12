CREATE TYPE [dbo].[TStudent] AS TABLE (
    [Id]                  INT            NOT NULL,
    [FirstName]           NVARCHAR (510) NOT NULL,
    [LastName]            NVARCHAR (510) NOT NULL,
    [BirthDate]           DATETIME2 (7)  NULL,
    [Gender]              NVARCHAR (510) NULL,
    [HasMedicalAlert]     BIT            NOT NULL,
    [IsAllowedInetAccess] BIT            NOT NULL,
    [SpecialInstructions] NVARCHAR (MAX) NOT NULL,
    [SpEdStatus]          NVARCHAR (512) NULL,
    [PhotoModifiedDate]   DATETIME2 (7)  NULL,
    [UserId]              INT            NOT NULL);

