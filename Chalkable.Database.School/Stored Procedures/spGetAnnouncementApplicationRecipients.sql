CREATE Procedure [dbo].[spGetAnnouncementApplicationRecipients] 
	@teacherId int, 
	@studentId int,
	@adminId int,
	@schoolYearId int,
	@appId uniqueidentifier
As

declare @selectedStudents table
	(
		selectedStudent int
	)

declare @teacherClassRefs table
	(
		teacherClassRef int,
		teacherClassName nvarchar(255),
		selectedStudentCount int
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
	set @teacherId = null
	insert into @teacherClassRefs
		select Class_Id, Class_Name, 1
			from vwClass
		where
			Class_SchoolYearRef = @schoolYearId
			and exists(select * from ClassPerson where ClassRef = Class_Id and PersonRef = @studentId)

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
	set @adminId = null;
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

	insert into @selectedStudents
	select
		distinct CP.PersonRef
	from
		ClassPerson CP
		join
		@teacherClassRefs TCR
			on TCR.teacherClassRef = CP.ClassRef
	where
		CP.IsEnrolled = 1

	insert into @resultTable 
	select
		AnnApp.annAppId, null, count(distinct SS.selectedStudent) as studentCount
	from
		@selectedStudents SS
		join StudentGroup SG
			on SG.StudentRef = SS.selectedStudent
		join AnnouncementGroup AG
			on AG.GroupRef = SG.GroupRef
		join @announcementApp AnnApp
			on AnnApp.announcementId = AG.AnnouncementRef
	group by AnnApp.annAppId
End

if(@adminId is not null)
Begin
	
	insert into @selectedStudents
	select
		distinct CP.PersonRef
	from
		ClassPerson CP
		join
		(select distinct StudentRef from [Group] G
			join StudentGroup SG on G.Id = SG.GroupRef
			where OwnerRef = @adminId
		) AdminStudent
			on AdminStudent.StudentRef = CP.PersonRef
	where
		CP.IsEnrolled = 1
	
	insert into @resultTable 
	select
		AnnApp.annAppId, null, count(distinct SS.selectedStudent) as studentCount
	from
		@selectedStudents SS
		join StudentGroup SG
			on SG.StudentRef = SS.selectedStudent
		join AnnouncementGroup AG
			on AG.GroupRef = SG.GroupRef
		join @announcementApp AnnApp
			on AnnApp.announcementId = AG.AnnouncementRef
	group by AnnApp.annAppId
	
End

--Get announcement ids for lesson plans
insert into @resultTable
select
	AnnApp.annAppId, LP.teacherClassName, LP.selectedStudentCount
from
	@announcementApp as AnnApp
	join (
			select 
				Id, 
				TCR.teacherClassName as teacherClassName, 
				TCR.teacherClassRef as teacherClassRef,
				TCR.selectedStudentCount as selectedStudentCount
			from 
				LessonPlan 
				join @teacherClassRefs as TCR 
					on LessonPlan.ClassRef = TCR.teacherClassRef
		) LP
	on AnnApp.announcementId = LP.Id

--Get announcement ids for class announcement
insert into @resultTable
select
	AnnApp.annAppId, CA.teacherClassName, CA.selectedStudentCount
from
	@announcementApp as AnnApp
	join 
		(
			select 
				Id, 
				TCR.teacherClassName as teacherClassName, 
				TCR.teacherClassRef as teacherClassRef,
				TCR.selectedStudentCount as selectedStudentCount
			from 
				ClassAnnouncement 
				join @teacherClassRefs as TCR 
					on ClassAnnouncement.ClassRef = TCR.teacherClassRef
		) CA
	on AnnApp.announcementId = CA.Id

--Get announcement ids for supplemental announcement
insert into @resultTable
select
	AnnApp.annAppId, SU.teacherClassName, SU.selectedStudentCount
from
	@announcementApp as AnnApp
	join 
		(
			select 
				Id, 
				TCR.teacherClassName as teacherClassName, 
				TCR.teacherClassRef as teacherClassRef,
				TCR.selectedStudentCount as selectedStudentCount
			from 
				SupplementalAnnouncement 
				join @teacherClassRefs as TCR 
					on SupplementalAnnouncement.ClassRef = TCR.teacherClassRef
		) SU
	on AnnApp.announcementId = SU.Id


select * from @resultTable

GO