alter VIEW [dbo].[vwAnnouncement] 
AS 
SELECT
	Announcement.Id as Id,
	Announcement.Created as Created,
	Announcement.Expires as Expires,
	Announcement.[State] as [State],
	Announcement.[Order] as [Order],
	Announcement.Content as Content,
	Announcement.[Subject] as [Subject],
	Announcement.Title as [Title],
	Announcement.GradingStyle as GradingStyle,
	Announcement.Dropped as Dropped,
	Announcement.ClassAnnouncementTypeRef as ClassAnnouncementTypeRef,
	Announcement.SchoolRef as SchoolRef,
	Announcement.SisActivityId as SisActivityId,
    Announcement.MaxScore as MaxScore,
    Announcement.WeightAddition as WeightAddition,
    Announcement.WeightMultiplier as WeightMultiplier,
    Announcement.MayBeDropped as MayBeDropped,
	Announcement.VisibleForStudent as VisibleForStudent,
	Announcement.ClassRef as ClassRef,
	Person.FirstName + ' ' + Person.LastName as PrimaryTeacherName,
	Person.Gender as PrimaryTeacherGender,
	Class.Name + ' ' + (case when Class.ClassNumber is null then '' else Class.ClassNumber end) as ClassName,
	Class.GradeLevelRef as GradeLevelId,
	Class.PrimaryTeacherRef as PrimaryTeacherRef,  
	(Select COUNT(*) from AnnouncementQnA where AnnouncementQnA.AnnouncementRef = Announcement.Id) as QnACount,	
	(Select COUNT(*) from ClassPerson where ClassRef = Announcement.ClassRef) as StudentsCount,
	(Select COUNT(*) from AnnouncementAttachment where AnnouncementRef = Announcement.Id) as AttachmentsCount,
	(select count(*) from AnnouncementAttachment where AnnouncementRef = Announcement.Id  and Class.PrimaryTeacherRef = PersonRef) as OwnerAttachmentsCount,
	(	select COUNT(*) from
			(Select distinct PersonRef from AnnouncementAttachment 
			 where AnnouncementRef = Announcement.Id and 
				   Announcement.ClassRef in (select ClassPerson.ClassRef from ClassPerson 
											 where ClassPerson.PersonRef = AnnouncementAttachment.PersonRef)) as x
	) as StudentsCountWithAttachments,
	(Select COUNT(*) from AnnouncementApplication where AnnouncementRef = Announcement.Id and Active = 1) as ApplicationCount

FROM 
	Announcement
	join Class on Class.Id = Announcement.ClassRef
	left join Person on Person.Id = Class.PrimaryTeacherRef


GO

