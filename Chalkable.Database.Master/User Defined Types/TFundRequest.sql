CREATE TYPE [dbo].[TFundRequest] AS TABLE (
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    [UserRef]       UNIQUEIDENTIFIER NULL,
    [CreatedByRef]  UNIQUEIDENTIFIER NOT NULL,
    [Amount]        MONEY            NOT NULL,
    [Created]       DATETIME2 (7)    NOT NULL,
    [PurchaseOrder] NVARCHAR (255)   NULL,
    [State]         INT              NULL,
    [SignatureRef]  UNIQUEIDENTIFIER NULL,
    [SchoolRef]     UNIQUEIDENTIFIER NOT NULL);

