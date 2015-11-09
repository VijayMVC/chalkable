Create Type TPrivateMessageRecipient As Table(
    PrivateMessageRef int not null,
    RecipientRef int not null,
    [Read] bit not null,
    DeletedByRecipient bit not null,
    RecipientClassRef int null
)
Go

