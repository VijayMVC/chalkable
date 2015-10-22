Create Procedure spGetRecipients
	@messageIds TInt32 ReadOnly
As

Select
	[Id] as PrivateMessageRef,
	[RecipientRef],
	[Read],
	[DeletedByRecipient],
	[RecipientClassRef],

	SenderId,
    SenderFirstName,
    SenderLastName,
    SenderSalutation,
    SenderRoleRef,
    SenderGender,
                                         
    RecipientId,
    RecipientFirstName,
    RecipientLastName,
    RecipientSalutation,
    RecipientRoleRef,
    RecipientGender,
                                         
    Name,
    ClassNumber
From
	vwPrivateMessage
Where
	Id in(select * from @messageIds)

Go