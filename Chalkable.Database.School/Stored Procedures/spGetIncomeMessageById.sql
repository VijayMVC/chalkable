

Create Procedure [dbo].[spGetIncomeMessageById]
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
SenderRoleRef,
SenderGender
from
vwIncomeMessage
where
RecipientRef = @personId
And DeletedByRecipient = 0
And Id = @messageId