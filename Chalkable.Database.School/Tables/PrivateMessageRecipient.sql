CREATE TABLE [dbo].[PrivateMessageRecipient] (
    [PrivateMessageRef]  INT NOT NULL,
    [RecipientRef]       INT NOT NULL,
    [RecipientClassRef]  INT NULL,
    [Read]               BIT NOT NULL,
    [DeletedByRecipient] BIT NOT NULL,
    CONSTRAINT [PK_PrivateMessageRecipient] PRIMARY KEY CLUSTERED ([PrivateMessageRef] ASC, [RecipientRef] ASC),
    CONSTRAINT [FK_PrivateMessageRecipient_Class] FOREIGN KEY ([RecipientClassRef]) REFERENCES [dbo].[Class] ([Id]),
    CONSTRAINT [FK_PrivateMessageRecipient_Person] FOREIGN KEY ([RecipientRef]) REFERENCES [dbo].[Person] ([Id]),
    CONSTRAINT [FK_PrivateMessageRecipient_PrivateMessage] FOREIGN KEY ([PrivateMessageRef]) REFERENCES [dbo].[PrivateMessage] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_PrivateMessageRecipient_RecipientClassRef
	ON dbo.PrivateMessageRecipient( RecipientClassRef )
GO


	
CREATE NONCLUSTERED INDEX IX_PrivateMessageRecipient_RecipientRef
	ON dbo.PrivateMessageRecipient( RecipientRef )
GO