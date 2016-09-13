CREATE PROCEDURE [dbo].[spGetStudentAnnouncementApplicationRecipients]
	@studentId int,
	@schoolYearId int,
	@appId uniqueidentifier
As
	declare @teacherClassRefs table
	(
		teacherClassRef int,
		teacherClassName nvarchar(255)
	)

declare 
	@announcementApp table
	(
		studentAnnAppId int,
		announcementId int
	)

insert into @announcementApp
	select 
		Id,
		AnnouncementRef
	From
		AnnouncementApplication
	Where 
		ApplicationRef = @appId 
		and
		Active = 1


insert into @teacherClassRefs
	select Class_Id, Class_Name
		from vwClass
	where
		Class_SchoolYearRef = @schoolYearId
		and exists(select * from ClassPerson where ClassRef = Class_Id and PersonRef = @studentId)


select
	distinct AnnApp.studentAnnAppId
from
	StudentGroup SG
	join AnnouncementGroup AG
		on AG.GroupRef = SG.GroupRef
	join @announcementApp AnnApp
		on AnnApp.announcementId = AG.AnnouncementRef
Where
	SG.StudentRef = @studentId

UNION

select
	distinct AnnApp.studentAnnAppId
from
	@announcementApp as AnnApp
	join (
			select 
				distinct Id
			from 
				LessonPlan 
				join @teacherClassRefs as TCR 
					on LessonPlan.ClassRef = TCR.teacherClassRef

			UNION

			select 
				distinct Id
			from 
				ClassAnnouncement 
				join @teacherClassRefs as TCR 
					on ClassAnnouncement.ClassRef = TCR.teacherClassRef

			UNION

			select 
				distinct Id
			from 
				SupplementalAnnouncement 
				join @teacherClassRefs as TCR 
					on SupplementalAnnouncement.ClassRef = TCR.teacherClassRef
		) Ann
	on AnnApp.announcementId = Ann.Id