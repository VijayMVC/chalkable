CREATE TABLE [dbo].[FundRequestRoleDistribution] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [RoleRef]        INT              NOT NULL,
    [FundRequestRef] UNIQUEIDENTIFIER NOT NULL,
    [Amount]         MONEY            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_FUND_ROLE_DEST_RUND_REQUETS] FOREIGN KEY ([FundRequestRef]) REFERENCES [dbo].[FundRequest] ([Id])
);

