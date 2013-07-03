
alter View [dbo].[vwAnnouncement] 
as 
Select 
	Announcement.Id as Id,
	Announcement.Created as Created,
	Announcement.Expires as Expires,
	Announcement.[State] as [State],
	Announcement.[Order] as [Order],
	Announcement.Content as Content,
	Announcement.[Subject] as [Subject],
	Announcement.GradingStyle as GradingStyle,
	Announcement.Dropped as Dropped,
	Announcement.AnnouncementTypeRef as AnnouncementTypeRef,
	AnnouncementType.Name as AnnouncementTypeName,
	Announcement.PersonRef as PersonRef,
	Announcement.MarkingPeriodClassRef as MarkingPeriodClassRef,
	Person.FirstName + ' ' + Person.LastName as PersonName,
	Person.Gender as PersonGender,
	Class.Name as ClassName,
	Class.GradeLevelRef as GradeLevelId,  
	Class.CourseRef as CourseId,
	MarkingPeriodClass.ClassRef as ClassId,
	MarkingPeriodClass.MarkingPeriodRef as MarkingPeriodId,
	(Select COUNT(*) from AnnouncementQnA where AnnouncementQnA.AnnouncementRef = Announcement.Id) as QnACount,	
	(Select COUNT(*) from ClassPerson where ClassRef = MarkingPeriodClass.ClassRef) as StudentsCount,
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
	--(Select top 1 (case when COUNT(aa.Id) = 1 then a.Name else null end) 
	--		from AnnouncementApplication aa 
	--		join Application a on  aa.ApplicationRef = a.Id 
	--		where aa.AnnouncementRef = Announcement.Id and aa.Active = 1
	--		group by a.Name) as ApplicationName
	--(select count(aa.Id) from AnnouncementApplication aa where aa.AnnouncementRef = Announcement.Id) as ApplicationName

from 
	Announcement
	join AnnouncementType on Announcement.AnnouncementTypeRef = AnnouncementType.Id
	left join MarkingPeriodClass on MarkingPeriodClass.Id = Announcement.MarkingPeriodClassRef
	left join Class on Class.Id = MarkingPeriodClass.ClassRef
	left join Person on Person.Id = Announcement.PersonRef

GO


