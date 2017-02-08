CREATE Procedure [dbo].[spGetAnnouncementRecipientPersons] 
	@announcementId int,
	@filterStudentType int,
	@teacherId int,
	@currentSchoolYearId int,
	@start int,	
	@count int
As

Declare @All int = 0,
		@MySchoolOnly int = 1,
		@MyStudentsOnly int = 2,

		@needFiltering bit = 0,
		@studentsIds TInt32;

If @filterStudentType = @MySchoolOnly
Begin
	Set @needFiltering = 1;

	Insert Into @studentsIds
		Select Distinct ClassPerson.PersonRef 
		From ClassPerson Join Class 
			On Class.Id = ClassPerson.ClassRef And Class.SchoolYearRef = @currentSchoolYearId
End
Else If @filterStudentType = @MyStudentsOnly
Begin
	Set @needFiltering = 1;

	Insert Into @studentsIds
		Select Distinct ClassPerson.PersonRef 
		From ClassTeacher join ClassPerson
				On ClassTeacher.ClassRef = classPerson.ClassRef And ClassTeacher.PersonRef = @teacherId
			Join Class On Class.Id = ClassPerson.ClassRef And ClassTeacher.ClassRef = Class.Id And class.SchoolYearRef = @currentSchoolYearId
End

Declare 
	@classId int,
	@schoolId int,
	@adminId int,
	@schoolYearId int

Select Top 1
	@classId = ann.ClassRef,
	@adminId = ann.AdminRef,
	@schoolYearId = ann.SchoolYearRef
From (
		Select
			LessonPlan.ClassRef,
			LessonPlan.SchoolYearRef,
			null as AdminRef
		From LessonPlan
		Where LessonPlan.Id = @announcementId
		
		Union
		
		Select 
			ClassAnnouncement.ClassRef, 
			ClassAnnouncement.SchoolYearRef, 
			null as AdminRef
		From ClassAnnouncement
		Where ClassAnnouncement.Id = @announcementId
		
		Union
		
		Select 
			null as ClassRef, 
			null as SchoolYearRef,
			AdminAnnouncement.AdminRef as AdminRef
		From AdminAnnouncement
		Where AdminAnnouncement.Id = @announcementId

		Union

		Select 
			null as ClassRef,  -- set ClassRef null to identify Supplemental
			SupplementalAnnouncement.SchoolYearRef,
			null as AdminRef
		From SupplementalAnnouncement
		Where SupplementalAnnouncement.Id = @announcementId
	) ann

Select Top 1 @schoolId = SchoolYear.SchoolRef
From SchoolYear
Where ID = @schoolYearId

If(@classId is not null)
Begin
	Select * From vwPerson
	Where Exists(select * from ClassPerson where ClassRef = @classId and PersonRef = vwPerson.Id) 
		And vwPerson.SchoolRef = @schoolId
		And (@needFiltering = 0 or vwPerson.Id in(Select * From @studentsIds))
	Order by vwPerson.Id
	OFFSET @start ROWS FETCH NEXT @count ROWS ONLY

End
Else
Begin
	if(@adminId is not null)
	Begin
		Select * 
		From vwPerson join StudentGroup
				On StudentGroup.StudentRef = vwPerson.Id 
			join AnnouncementGroup
				On AnnouncementGroup.GroupRef = StudentGroup.GroupRef And AnnouncementGroup.AnnouncementRef = @announcementId
		Where (@needFiltering = 0 or vwPerson.Id in(Select * From @studentsIds))
		Order by vwPerson.Id
		OFFSET @start ROWS FETCH NEXT @count ROWS ONLY
	End
	Else
	Begin
		--normally Supplemental has ClassRef but in this case above I set ClassRef null, that need to identify Supplemental
		Select * From
			vwPerson join SupplementalAnnouncementRecipient
				On SupplementalAnnouncementRecipient.StudentRef = vwPerson.Id 
		Where SupplementalAnnouncementRecipient.SupplementalAnnouncementRef = @announcementId 
			And vwPerson.SchoolRef = @schoolId
			And (@needFiltering = 0 or vwPerson.Id in(Select * From @studentsIds))
		Order by vwPerson.Id
		OFFSET @start ROWS FETCH NEXT @count ROWS ONLY
	End
End