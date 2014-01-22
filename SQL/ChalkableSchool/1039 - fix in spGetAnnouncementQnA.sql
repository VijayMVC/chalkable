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
sp2.Id as AnswererId,
sp2.FirstName as AnswererFirstName,
sp2.LastName as AnswererLastName,
sp2.Gender as AnswererGender
from AnnouncementQnA ana
join vwPerson sp1 on sp1.Id = ana.PersonRef
join Announcement a on a.Id = ana.AnnouncementRef
join vwPerson sp2 on sp2.Id = a.PersonRef
GO

ALTER procedure [dbo].[spGetAnnouncementsQnA] @callerId uniqueidentifier, @announcementQnAId uniqueidentifier
, @announcementId uniqueidentifier, @askerId uniqueidentifier, @answererId uniqueidentifier
as
declare @callerRolerId int = (select RoleRef from Person where Id = @callerId)
select vwAnnouncementQnA.*,
cast((case when @callerId = vwAnnouncementQnA.AskerId then 1 else 0 end) as bit) as IsOwner
from vwAnnouncementQnA
where (@announcementId is null or AnnouncementRef = @announcementId)
and (@askerId is null or @askerId = AskerId)
and (@answererId is  null or @answererId = AnswererId)
and (@callerRolerId = 1 or @callerId = AnswererId  or @callerId = AskerId or
(MarkingPeriodClassRef is not null and AnsweredTime is not null
and exists(select * from MarkingPeriodClass mpc
join ClassPerson csp on csp.ClassRef = mpc.ClassRef
where mpc.Id = MarkingPeriodClassRef and @callerId = csp.PersonRef
)
)
)
and (@announcementQnAId is null or @announcementQnAId = Id)
GO


