CREATE Procedure [dbo].[spGetAnnouncementApplicationRecipients] 
	@teacherId int, 
	@studentId int,
	@schoolYearId int,
	@appId uniqueidentifier
As
declare @teacherStudents table
	(
		teacherStudent int
	)

declare @teacherClassRefs table
	(
		teacherClassRef int,
		teacherClassName nvarchar(255),
		teacherStudentCount int
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
		AnnouncementAppicationId int,
		ClassName nvarchar(255),
		StudentCount int
	)
--Get all announcement application for application
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

if(@studentId is not null)
Begin 
	--get teacher classes by student (prepare data for lesson plans and class announcement)
	set @teacherId = null
	insert into @teacherClassRefs
		select Class_Id, Class_Name, 1
			from vwClass
		where
			Class_SchoolYearRef = @schoolYearId
			and exists(select * from ClassPerson where ClassRef = Class_Id and PersonRef = @studentId)

	--Get data for admin announcement
	insert into @resultTable 
		select
			AnnApp.annAppId, null, 1
		from
			StudentGroup SG
			join AnnouncementGroup AG
				on AG.GroupRef = SG.GroupRef
			join @announcementApp AnnApp
				on AnnApp.announcementId = AG.AnnouncementRef
		Where
			SG.StudentRef = @studentId
		group by AnnApp.annAppId
End

if(@teacherId is not null)
Begin
	--get teacher classes by teacher (prepare data for lesson plans and class announcement)
	insert into @teacherClassRefs
	Select 
		distinct ClassTeacher.ClassRef, Class.Name, (select count(distinct PersonRef) from ClassPerson where ClassRef = ClassTeacher.ClassRef)
	from 
		ClassPerson 
		join ClassTeacher 
			on ClassTeacher.ClassRef = ClassPerson.ClassRef
		join Class
			on ClassTeacher.ClassRef = Class.Id
		join StudentSchoolYear 
			on StudentSchoolYear.StudentRef = Classperson.PersonRef
	Where 
		StudentSchoolYear.SchoolYearRef =  @schoolYearId 
		and ClassTeacher.PersonRef = @teacherId

	--get teacher students (need for admin announcement data)
	insert into @teacherStudents
	select
		distinct CP.PersonRef
	from
		ClassPerson CP
		join
		@teacherClassRefs TCR
			on TCR.teacherClassRef = CP.ClassRef
	where
		CP.IsEnrolled = 1

	--Get data for admin announcement
	insert into @resultTable 
	select
		AnnApp.annAppId, null, count(distinct TS.teacherStudent) as studentCount
	from
		@teacherStudents TS
		join StudentGroup SG
			on SG.StudentRef = TS.teacherStudent
		join AnnouncementGroup AG
			on AG.GroupRef = SG.GroupRef
		join @announcementApp AnnApp
			on AnnApp.announcementId = AG.AnnouncementRef
	group by AnnApp.annAppId
End

--Get data for lesson plans
insert into @resultTable
select
	AnnApp.annAppId, LP.teacherClassName, LP.teacherStudentCount
from
	@announcementApp as AnnApp
	join (
			select 
				Id, 
				TCR.teacherClassName as teacherClassName, 
				TCR.teacherClassRef as teacherClassRef,
				TCR.teacherStudentCount as teacherStudentCount
			from 
				LessonPlan 
				join @teacherClassRefs as TCR 
					on LessonPlan.ClassRef = TCR.teacherClassRef
		) LP
	on AnnApp.announcementId = LP.Id

--Get data for class announcement
insert into @resultTable
select
	AnnApp.annAppId, CA.teacherClassName, CA.teacherStudentCount
from
	@announcementApp as AnnApp
	join 
		(
			select 
				Id, 
				TCR.teacherClassName as teacherClassName, 
				TCR.teacherClassRef as teacherClassRef,
				TCR.teacherStudentCount as teacherStudentCount
			from 
				ClassAnnouncement 
				join @teacherClassRefs as TCR 
					on ClassAnnouncement.ClassRef = TCR.teacherClassRef
		) CA
	on AnnApp.announcementId = CA.Id

select * from @resultTable