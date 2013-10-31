create VIEW [vwPerson]
AS
SELECT
	Person.Id as Id,
	SchoolPerson.RoleRef as RoleRef,
	Person.FirstName as FirstName,
	Person.LastName as LastName,
	Person.BirthDate as BirthDate,
	Person.Gender as Gender,
	Person.Salutation as Salutation,
	Person.Active as Active,
	Person.FirstLoginDate as FirstLogInDate,
	Person.Email as Email,
	GradeLevel.Id as GradeLevel_Id,
	GradeLevel.Name as GradeLevel_Name
FROM 
	Person
	Join SchoolPerson on Person.Id = SchoolPerson.PersonRef
	left join StudentSchoolYear on StudentSchoolYear.StudentRef = Person.Id
	left join GradeLevel on StudentSchoolYear.GradeLevelRef = GradeLevel.Id	
	
GO

create VIEW [vwAnnouncement] 
AS 
SELECT
	Announcement.Id as Id,
	Announcement.CREATEd as CREATEd,
	Announcement.Expires as Expires,
	Announcement.[State] as [State],
	Announcement.[Order] as [Order],
	Announcement.Content as Content,
	Announcement.[Subject] as [Subject],
	Announcement.GradingStyle as GradingStyle,
	Announcement.Dropped as Dropped,
	Announcement.ClassAnnouncementTypeRef as ClassAnnouncementTypeRef,
	Announcement.SchoolRef as SchoolRef,
	ClassAnnouncementType.Name as ClassAnnouncementTypeName,
	Announcement.PersonRef as PersonRef,
	Announcement.ClassRef as ClassRef,
	Person.FirstName + ' ' + Person.LastName as PersonName,
	Person.Gender as PersonGender,
	Class.Name as ClassName,
	Class.GradeLevelRef as GradeLevelId,  
	(Select COUNT(*) from AnnouncementQnA where AnnouncementQnA.AnnouncementRef = Announcement.Id) as QnACount,	
	(Select COUNT(*) from ClassPerson where ClassRef = Announcement.ClassRef) as StudentsCount,
	(Select COUNT(*) from AnnouncementAttachment where AnnouncementRef = Announcement.Id) as AttachmentsCount,
	(select count(*) from AnnouncementAttachment where AnnouncementRef = Announcement.Id and PersonRef = Announcement.PersonRef) as OwnerAttachmentsCount,
	(	select COUNT(*) from
			(Select distinct PersonRef from AnnouncementAttachment 
			where AnnouncementRef = Announcement.Id
			and PersonRef <> Announcement.PersonRef) as x
	) as StudentsCountWithAttachments,
	(Select COUNT(*) from StudentAnnouncement where AnnouncementRef = Announcement.Id and GradeValue is not null) as GradingStudentsCount, 
	(Select AVG(GradeValue) from StudentAnnouncement where AnnouncementRef = Announcement.Id and GradeValue is not null) as [Avg], 
	(Select COUNT(*) from AnnouncementApplication where AnnouncementRef = Announcement.Id and Active = 1) as ApplicationCount

FROM 
	Announcement
	left join ClassAnnouncementType on Announcement.ClassAnnouncementTypeRef = ClassAnnouncementType.Id
	left join Class on Class.Id = Announcement.ClassRef
	left join Person on Person.Id = Announcement.PersonRef
GO


create VIEW [dbo].[vwClass]
AS
SELECT
	Class.Id as Class_Id,
	Class.Name as Class_Name,
	Class.Description as Class_Description,
	Class.SchoolYearRef as Class_SchoolYearRef,
	Class.TeacherRef as Class_TeacherRef,
	Class.GradeLevelRef as Class_GradeLevelRef,
	Class.ChalkableDepartmentRef as Class_ChalkableDepartmentId,
	Class.SchoolRef as Class_SchoolRef,
	GradeLevel.Id as GradeLevel_Id,
	GradeLevel.Name as GradeLevel_Name,
	GradeLevel.Number as GradeLevel_Number,
	Person.Id as Person_Id,
	Person.FirstName as Person_FirstName,
	Person.LastName as Person_LastName,
	Person.Gender as Person_Gender,
	Person.Salutation as Person_Salutation,
	Person.Email as Person_Email,
	SchoolYear.SchoolRef as Class_SchoolId 
FROM 
	Class	
	join GradeLevel on GradeLevel.Id = Class.GradeLevelRef
	join Person on Person.Id = Class.TeacherRef
	join SchoolYear on SchoolYear.Id = Class.SchoolYearRef
GO


CREATE VIEW [vwAnnouncementQnA]
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
	FROM
		AnnouncementQnA ana
		join vwPerson sp1 on sp1.Id = ana.PersonRef
		join Announcement a on a.Id = ana.AnnouncementRef
		join vwPerson sp2 on sp2.Id = a.PersonRef
GO



CREATE VIEW vwPrivateMessage
AS
	SELECT
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
		 sp.RoleRef as PrivateMessage_SenderRoleRef,
		 p2.FirstName as PrivateMessage_RecipientFirstName,
		 p2.LastName as PrivateMessage_RecipientLastName,
		 p2.Gender as PrivateMessage_RecipientGender,
		 p2.Salutation as PrivateMessage_RecipientSalutation,
		 sp2.RoleRef as PrivateMessage_RecipientRoleRef
	FROM 
		PrivateMessage 
		join Person p on p.Id = PrivateMessage.FromPersonRef
		join SchoolPerson sp on sp.PersonRef = p.Id
		join Person p2 on p2.Id = PrivateMessage.ToPersonRef 
		join SchoolPerson sp2 on sp2.PersonRef = p2.Id
GO 


