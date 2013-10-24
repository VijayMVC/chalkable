alter procedure [dbo].[spGetAnnouncementsQnA] @callerId uniqueidentifier, @announcementQnAId uniqueidentifier
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
order by QuestionTime
GO


