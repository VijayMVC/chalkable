Create Procedure [dbo].[spGetMessageRecipients]
@messageIds TInt32 ReadOnly
As

Select
pmr.PrivateMessageRef,
pmr.[RecipientRef],
pmr.[Read],
pmr.[DeletedByRecipient],
pmr.[RecipientClassRef],

SenderId,
SenderFirstName,
SenderLastName,
SenderRoleRef,
SenderGender,

RecipientId,
RecipientFirstName,
RecipientLastName,
RecipientRoleRef,
RecipientGender,

Name,
ClassNumber
From
PrivateMessage pm
join PrivateMessageRecipient pmr
on pm.Id = pmr.PrivateMessageRef
join (Select
[Id] as RecipientId,
[FirstName] as RecipientFirstName,
[LastName] as RecipientLastName,
3 as RecipientRoleRef,
[Gender] as RecipientGender
From
Student
Union

Select
Staff.[Id] as RecipientId,
Staff.[FirstName] as RecipientFirstName,
Staff.[LastName] as RecipientLastName,
2 as RecipientRoleRef,
Staff.[Gender] as RecipientGender
From  Staff) as recipient
on recipient.RecipientId = pmr.RecipientRef
join (Select
[Id] as SenderId,
[FirstName] as SenderFirstName,
[LastName] as SenderLastName,
3 as SenderRoleRef,
[Gender] as SenderGender
From
Student
Union

Select
Staff.[Id] as SenderId,
Staff.[FirstName] as SenderFirstName,
Staff.[LastName] as SenderLastName,
2 as SenderRoleRef,
Staff.[Gender] as SenderGender
From Staff) as sender
on sender.SenderId = pm.FromPersonRef
Left Join Class
On Class.Id = pmr.RecipientClassRef
Where
pm.Id in(select * from @messageIds)