CREATE TYPE [dbo].[TFund] AS TABLE (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [PerformedDateTime]   DATETIME2 (7)    NOT NULL,
    [Amount]              MONEY            NOT NULL,
    [Description]         NVARCHAR (MAX)   NULL,
    [FromSchoolRef]       UNIQUEIDENTIFIER NULL,
    [ToSchoolRef]         UNIQUEIDENTIFIER NULL,
    [FromUserRef]         UNIQUEIDENTIFIER NULL,
    [ToUserRef]           UNIQUEIDENTIFIER NULL,
    [AppInstallActionRef] UNIQUEIDENTIFIER NULL,
    [IsPrivate]           BIT              NOT NULL,
    [FundRequestRef]      UNIQUEIDENTIFIER NULL,
    [SchoolRef]           UNIQUEIDENTIFIER NOT NULL);

