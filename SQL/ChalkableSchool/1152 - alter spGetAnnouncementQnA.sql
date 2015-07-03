ALTER VIEW [dbo].[vwAnnouncementQnA]
AS
	SELECT
		ana.id as Id,
		ana.AnsweredTime as AnsweredTime,
		ana.QuestionTime as QuestionTime,
		ana.Question as Question,
		ana.Answer as Answer,
		ana.AnnouncementRef as AnnouncementRef,
		a.ClassRef as ClassRef,
		a.AdminRef as AdminRef,
		ana.State as [State],
		st.Id as AskerId,
		st.FirstName as AskerFirstName,
		st.LastName as AskerLastName,
		st.Gender as AskerGender,
		cast(3 as int) as AskerRoleRef,
		sts.SchoolRef as AskerSchoolRef,
		sf.Id as AnswererId,
		sf.FirstName as AnswererFirstName,
		sf.LastName as AnswererLastName,
		sf.Gender as AnswererGender,
		cast(2 as int) as AnswererRoleRef,
		sfs.SchoolRef as AnswererSchoolRef
	FROM
		AnnouncementQnA ana
		join Student st on st.Id = ana.AskerRef
		join StudentSchool sts on sts.StudentRef = st.Id
		join Announcement a on a.Id = ana.AnnouncementRef
		left join Staff sf on sf.Id = ana.AnswererRef
		left join StaffSchool sfs on sfs.StaffRef = sf.Id
GO

ALTER procedure [dbo].[spGetAnnouncementsQnA] @callerId int, @announcementQnAId int, @announcementId int
											, @askerId int, @answererId int, @schoolId int
as
	declare @callerRolerId int = (select top 1 RoleRef from SchoolPerson where PersonRef = @callerId)

	select vwAnnouncementQnA.*,
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

GO


