Alter View vwPrivateMessage
As

Select  distinct 
	PrivateMessage.[Id],
	PrivateMessage.[FromPersonRef],
	PrivateMessage.[Sent],
	PrivateMessage.[Subject],
	PrivateMessage.[Body],
	PrivateMessage.[DeletedBySender],

	PrivateMessageRecipient.[RecipientRef], 
	PrivateMessageRecipient.[RecipientClassRef], 
	PrivateMessageRecipient.[Read], 
	PrivateMessageRecipient.[DeletedByRecipient],

    vwPerson.Id as SenderId,
    vwPerson.FirstName as SenderFirstName,
    vwPerson.LastName as SenderLastName,
    vwPerson.Salutation as SenderSalutation,
    vwPerson.RoleRef as SenderRoleRef,
    vwPerson.Gender as SenderGender,
	--vwPerson.SchoolRef as SenderSchool,
                                         
    recipient.Id as RecipientId,
    recipient.FirstName as RecipientFirstName,
    recipient.LastName as RecipientLastName,
    recipient.Salutation as RecipientSalutation,
    recipient.RoleRef as RecipientRoleRef,
    recipient.Gender as RecipientGender,
	--recipient.SchoolRef as RecipientSchool,
                                         
    Class.Name as Name,
    Class.ClassNumber as ClassNumber
From 
	PrivateMessage 
	JOIN [vwPerson] 
		ON [vwPerson].[Id] = [PrivateMessage].[FromPersonRef] 
	JOIN [PrivateMessageRecipient] 
		ON [PrivateMessageRecipient].[PrivateMessageRef] = [PrivateMessage].[Id]  
	Join vwPerson as recipient 
		On recipient.Id = PrivateMessageRecipient.RecipientRef 
	Left Join Class 
		On Class.Id = PrivateMessageRecipient.RecipientClassRef

Go