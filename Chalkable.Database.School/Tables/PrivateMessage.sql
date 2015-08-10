CREATE TABLE [dbo].[PrivateMessage] (
    [Id]                 INT             IDENTITY (1, 1) NOT NULL,
    [FromPersonRef]      INT             NOT NULL,
    [ToPersonRef]        INT             NOT NULL,
    [Sent]               DATETIME2 (7)   NULL,
    [Subject]            NVARCHAR (1024) NOT NULL,
    [Body]               NVARCHAR (MAX)  NOT NULL,
    [Read]               BIT             NOT NULL,
    [DeletedBySender]    BIT             NOT NULL,
    [DeletedByRecipient] BIT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PrivateMessage_FromPerson] FOREIGN KEY ([FromPersonRef]) REFERENCES [dbo].[Person] ([Id]),
    CONSTRAINT [FK_PrivateMessage_ToPerson] FOREIGN KEY ([ToPersonRef]) REFERENCES [dbo].[Person] ([Id])
);

