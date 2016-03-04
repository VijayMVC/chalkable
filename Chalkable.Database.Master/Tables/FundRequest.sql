CREATE TABLE [dbo].[FundRequest] (
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    [UserRef]       UNIQUEIDENTIFIER NULL,
    [CreatedByRef]  UNIQUEIDENTIFIER NOT NULL,
    [Amount]        MONEY            NOT NULL,
    [Created]       DATETIME2 (7)    NOT NULL,
    [PurchaseOrder] NVARCHAR (255)   NULL,
    [State]         INT              NULL,
    [SignatureRef]  UNIQUEIDENTIFIER NULL,
    [SchoolRef]     UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_FUND_REQUEST_CREATED_BY] FOREIGN KEY ([CreatedByRef]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_FUND_REQUEST_USER] FOREIGN KEY ([UserRef]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_FundRequest_School] FOREIGN KEY ([SchoolRef]) REFERENCES [dbo].[School] ([Id])
);

