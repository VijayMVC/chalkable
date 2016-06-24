CREATE TYPE [dbo].[TPrivateMessage] AS TABLE (
    [Id]                 INT             NOT NULL,
    [FromPersonRef]      INT             NOT NULL,
    [ToPersonRef]        INT             NOT NULL,
    [Sent]               DATETIME2 (7)   NULL,
    [Subject]            NVARCHAR (1024) NOT NULL,
    [Body]               NVARCHAR (MAX)  NOT NULL,
    [Read]               BIT             NOT NULL,
    [DeletedBySender]    BIT             NOT NULL,
    [DeletedByRecipient] BIT             NOT NULL);

