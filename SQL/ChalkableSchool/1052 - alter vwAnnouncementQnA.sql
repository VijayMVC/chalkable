alter VIEW [dbo].[vwAnnouncementQnA]
AS
	SELECT
		ana.id as Id,
		ana.AnsweredTime as AnsweredTime,
		ana.QuestionTime as QuestionTime,
		ana.Question as Question,
		ana.Answer as Answer,
		ana.AnnouncementRef as AnnouncementRef,
		a.ClassRef as ClassRef,
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

