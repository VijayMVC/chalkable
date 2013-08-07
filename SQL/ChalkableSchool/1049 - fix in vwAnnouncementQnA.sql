ALTER view [dbo].[vwAnnouncementQnA]
as
select
ana.id as Id,
ana.AnsweredTime as AnsweredTime,
ana.QuestionTime as QuestionTime,
ana.Question as Question,
ana.Answer as Answer,
ana.AnnouncementRef as AnnouncementRef,
a.MarkingPeriodClassRef as MarkingPeriodClassRef,
ana.State as [State],
sp1.Id as AskerId,
sp1.FirstName as AskerFirstName,
sp1.LastName as AskerLastName,
sp1.Gender as AskerGender,
sp1.RoleRef as AskerRoleRef,
sp2.Id as AnswererId,
sp2.FirstName as AnswererFirstName,
sp2.LastName as AnswererLastName,
sp2.Gender as AnswererGender,
sp2.RoleRef as AnswererRoleRef
from AnnouncementQnA ana
join vwPerson sp1 on sp1.Id = ana.PersonRef
join Announcement a on a.Id = ana.AnnouncementRef
join vwPerson sp2 on sp2.Id = a.PersonRef
GO

ALTER view [dbo].[vwPrivateMessage]
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
	 p.RoleRef as PrivateMessage_SenderRoleRef,
	 p2.FirstName as PrivateMessage_RecipientFirstName,
	 p2.LastName as PrivateMessage_RecipientLastName,
	 p2.Gender as PrivateMessage_RecipientGender,
	 p2.Salutation as PrivateMessage_RecipientSalutation,
	 p2.RoleRef as PrivateMessage_RecipientRoleRef
from PrivateMessage 
join Person p on p.Id = PrivateMessage.FromPersonRef
join Person p2 on p2.Id = PrivateMessage.ToPersonRef 
GO