Create Procedure spGetIncomeMessageById
@personId int,
@messageId int
As

select top 1
	[Id],
	[FromPersonRef],
	[Sent],
	[Subject],
	[Body],
	[DeletedBySender],
	[Read],
	[DeletedByRecipient],
	SenderId,
    SenderFirstName,
    SenderLastName,
    SenderSalutation,
    SenderRoleRef,
    SenderGender
from 
	vwPrivateMessage
where 
	RecipientRef = @personId
	And DeletedByRecipient = 0
	And Id = @messageId

Go