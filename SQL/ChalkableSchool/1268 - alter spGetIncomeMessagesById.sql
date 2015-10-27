Alter Procedure [dbo].[spGetIncomeMessageById]
@personId int,
@schoolYearId int,
@messageId int
As

select top 1
	[Id],
	[FromPersonRef],
	[Sent],
	[Subject],
	[Body],
	[DeletedBySender],
	[SchoolYearRef],
	[Read],
	[DeletedByRecipient],
	SenderId,
    SenderFirstName,
    SenderLastName,
    SenderRoleRef,
    SenderGender
from 
	vwPrivateMessage
where 
	RecipientRef = @personId
	And DeletedByRecipient = 0
	And Id = @messageId
	And SchoolYearRef = @schoolYearId

GO