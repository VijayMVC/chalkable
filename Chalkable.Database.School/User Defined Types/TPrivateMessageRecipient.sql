CREATE TYPE [dbo].[TPrivateMessageRecipient] AS TABLE (
    [PrivateMessageRef]  INT NOT NULL,
    [RecipientRef]       INT NOT NULL,
    [Read]               BIT NOT NULL,
    [DeletedByRecipient] BIT NOT NULL,
    [RecipientClassRef]  INT NULL);

