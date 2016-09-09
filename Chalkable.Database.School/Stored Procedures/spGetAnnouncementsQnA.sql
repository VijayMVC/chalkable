
CREATE procedure [dbo].[spGetAnnouncementsQnA] @callerId int, @announcementQnAId int, @announcementId int
, @askerId int, @answererId int, @schoolId int
as
declare @callerRolerId int = (select top 1 RoleRef from SchoolPerson where PersonRef = @callerId)


select distinct
vwAnnouncementQnA.Id,
vwAnnouncementQnA.AnsweredTime,
vwAnnouncementQnA.QuestionTime,
vwAnnouncementQnA.Question,
vwAnnouncementQnA.Answer,
vwAnnouncementQnA.AnnouncementRef,
vwAnnouncementQnA.ClassRef,
vwAnnouncementQnA.AdminRef,
vwAnnouncementQnA.[State],
vwAnnouncementQnA.AskerId,
vwAnnouncementQnA.AskerFirstName,
vwAnnouncementQnA.AskerLastName,
vwAnnouncementQnA.AskerGender,
vwAnnouncementQnA.AskerRoleRef,
vwAnnouncementQnA.AnswererId,
vwAnnouncementQnA.AnswererFirstName,
vwAnnouncementQnA.AnswererLastName,
vwAnnouncementQnA.AnswererGender,
vwAnnouncementQnA.AnswererRoleRef,

cast((case when @callerId = vwAnnouncementQnA.AskerId then 1 else 0 end) as bit) as IsOwner

from vwAnnouncementQnA
where (@announcementId is null or AnnouncementRef = @announcementId)
and (@askerId is null or @askerId = AskerId)
and (@answererId is  null or @answererId = AnswererId)
and (@callerRolerId = 1 or @callerId = AnswererId  or @callerId = AskerId
or 	(ClassRef is not null and AnsweredTime is not null
and exists(select * from ClassPerson cp where cp.ClassRef = ClassRef and @callerId = cp.PersonRef)
)
or (ClassRef is not null and exists(select * from ClassTeacher ct where ct.ClassRef = ClassRef and @callerId = ct.PersonRef))
or (AdminRef is not null and AdminRef = @callerId)
)
and (@announcementQnAId is null or @announcementQnAId = Id)
and (AdminRef is not null or (AskerSchoolRef = @schoolId and (AnswererSchoolRef is null or AnswererSchoolRef = @schoolId)))
order by QuestionTime