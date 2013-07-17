create view vwPrivateMessage
as
select
	 PrivateMessage.Id as PrivateMessage_Id,
	 PrivateMessage.Body as PrivateMessage_Body,
	 PrivateMessage.[Read] as PrivateMessage_Read,
	 PrivateMessage.[Sent] as PrivateMessage_Sent,
	 PrivateMessage.[Subject] as PrivateMessage_Subject,
	 PrivateMessage.DeletedBySender as PrivateMessage_DeletedBySender,
	 PrivateMessage.DeletedByRecipient as PrivateMessage_DeletedByRecipient,
	 PrivateMessage.ToPersonRef as PrivateMessage_ToPersonRef,
	 PrivateMessage.FromPersonRef as PrivateMessage_FromPersonRef,
	 p.FirstName as PrivateMessage_SenderFirstName,
	 p.LastName as PrivateMessage_SenderLastName,
	 p.Gender as PrivateMessage_SenderGender,
	 p.Salutation as PrivateMessage_SenderSalutation,
	 p.RoleRef as PrivateMessage_SenderRoleId,
	 p2.FirstName as PrivateMessage_RecipientFirstName,
	 p2.LastName as PrivateMessage_RecipientLastName,
	 p2.Gender as PrivateMessage_RecipientGender,
	 p2.Salutation as PrivateMessage_RecipientSalutation,
	 p2.RoleRef as PrivateMessage_RecipientRoleId
from PrivateMessage 
join Person p on p.Id = PrivateMessage.FromPersonRef
join Person p2 on p2.Id = PrivateMessage.ToPersonRef 

