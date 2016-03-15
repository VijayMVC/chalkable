CREATE TYPE [dbo].[TFundRequestRoleDistribution] AS TABLE (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [RoleRef]        INT              NOT NULL,
    [FundRequestRef] UNIQUEIDENTIFIER NOT NULL,
    [Amount]         MONEY            NOT NULL);

