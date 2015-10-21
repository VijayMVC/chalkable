

Create View [dbo].[vwIncomeMessage]
as

Select distinct PrivateMessage.*, 
				RecipientRef,
                [Read], 
                DeletedByRecipient,
                vwPerson.Id as SenderId,
                vwPerson.FirstName as SenderFirstName,
                vwPerson.LastName as SenderLastName,
                vwPerson.Salutation as SenderSalutation,
                vwPerson.RoleRef as SenderRoleRef,
                vwPerson.Gender as SenderGender
From 
	PrivateMessage JOIN [vwPerson] 
		ON [vwPerson].[Id] = [PrivateMessage].[FromPersonRef] 
	JOIN [PrivateMessageRecipient] 
		ON [PrivateMessageRecipient].[PrivateMessageRef] = [PrivateMessage].[Id]


GO


