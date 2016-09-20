


CREATE View [dbo].[vwAnnouncementQnA]
As
Select
ana.id as Id,
ana.AnsweredTime as AnsweredTime,
ana.QuestionTime as QuestionTime,
ana.Question as Question,
ana.Answer as Answer,
ana.AnnouncementRef as AnnouncementRef,
(case 
	when ca.ClassRef is not null then ca.ClassRef 
	when lp.ClassRef is not null then lp.ClassRef
	else sa.ClassRef end) as ClassRef,
aa.AdminRef as AdminRef,

ana.[State] as [State],
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
From
AnnouncementQnA ana
join Student st on st.Id = ana.AskerRef
join StudentSchool sts on sts.StudentRef = st.Id
left join LessonPlan lp on lp.Id = ana.AnnouncementRef
left join ClassAnnouncement ca on ca.Id = ana.AnnouncementRef
left join SupplementalAnnouncement sa on sa.Id = ana.AnnouncementRef
left join AdminAnnouncement aa on aa.Id = ana.AnnouncementRef
left join Staff sf on sf.Id = ana.AnswererRef
left join StaffSchool sfs on sfs.StaffRef = sf.Id