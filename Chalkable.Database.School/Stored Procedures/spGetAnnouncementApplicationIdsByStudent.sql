CREATE Procedure [dbo].[spGetAnnouncementApplicationIdsByStudent] @studentId int, @appId uniqueidentifier, @schoolYear int
As
declare 
	@studentClassRefs table
	(
		studentClassRef int
	)

declare 
	@announcementApp table
	(
		annAppId int,
		announcementId int
	)

declare
	@resultTable table
	(
		annAppId int
	)

--Get student classes
insert into @studentClassRefs
select 
	distinct ClassPerson.ClassRef
From
	Student
	join StudentSchoolYear
		on Student.Id = StudentSchoolYear.StudentRef
	join ClassPerson
		on Student.Id = ClassPerson.PersonRef
Where
	(@studentId is null or Student.Id = @studentId)
	and
	(@schoolYear is null or StudentSchoolYear.SchoolYearRef = @schoolYear)

--Get announcement ids for application
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

--Get announcement ids for lesson plans
insert into @resultTable
select
	AnnApp.annAppId
from
	@announcementApp as AnnApp
	join (select Id from LessonPlan join @studentClassRefs as SCR on LessonPlan.ClassRef = SCR.studentClassRef) as LP
	on AnnApp.announcementId = LP.Id

--Get announcement ids for class announcement
insert into @resultTable
select
	AnnApp.annAppId
from
	@announcementApp as AnnApp
	join (select Id from ClassAnnouncement join @studentClassRefs as SCR on ClassAnnouncement.ClassRef = SCR.studentClassRef) as LP
	on AnnApp.announcementId = LP.Id

--Get announcement ids for admin announcement
insert into @resultTable
select
	AnnApp.annAppId
from
	@announcementApp as AnnApp
	join AnnouncementGroup 
	on AnnApp.announcementId = AnnouncementGroup.AnnouncementRef
	join (select GroupRef from StudentGroup where StudentRef = @studentId) as SG 
	on AnnouncementGroup.GroupRef = SG.GroupRef

select distinct annAppId from @resultTable