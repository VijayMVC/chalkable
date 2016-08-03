CREATE Procedure [dbo].[spGetAnnouncementRecipientPersons] @announcementId int, @start int,	@count int
As
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
		From
			LessonPlan
		Where
			LessonPlan.Id = @announcementId
		
		Union
		
		Select
			ClassAnnouncement.ClassRef, ClassAnnouncement.SchoolYearRef, null as AdminRef
		From
			ClassAnnouncement
		Where
			ClassAnnouncement.Id = @announcementId
		
		Union
		
		Select
			null as ClassRef, null as SchoolYearRef, AdminAnnouncement.AdminRef as AdminRef
		From
			AdminAnnouncement
		Where
			AdminAnnouncement.Id = @announcementId

		Union

		Select
			null as ClassRef, SupplementalAnnouncement.SchoolYearRef, null as AdminRef
		From
			SupplementalAnnouncement
		Where
			SupplementalAnnouncement.Id = @announcementId
	) ann

Select Top 1
	@schoolId = SchoolYear.SchoolRef
From
	SchoolYear
Where
	ID = @schoolYearId

If(@classId is not null)
	Begin
		Select
			*
		From
			vwPerson
		Where
			exists(select * from ClassPerson where ClassRef = @classId and PersonRef = vwPerson.Id) and vwPerson.SchoolRef = @schoolId
		Order by 
			vwPerson.Id
			OFFSET @start ROWS FETCH NEXT @count ROWS ONLY
	End
Else
	Begin
		if(@adminId is not null)
		Begin
			Select
				*
			From
				vwPerson 
				join StudentGroup
					On StudentGroup.StudentRef = vwPerson.Id 
				join AnnouncementGroup
					On AnnouncementGroup.GroupRef = StudentGroup.GroupRef and AnnouncementGroup.AnnouncementRef = @announcementId
			Order by 
				vwPerson.Id
				OFFSET @start ROWS FETCH NEXT @count ROWS ONLY
		End
		Else
		Begin
			Select
				*
			From
				vwPerson 
				join SupplementalAnnouncementRecipient
					On SupplementalAnnouncementRecipient.StudentRef = vwPerson.Id 
			Where
				SupplementalAnnouncementRecipient.SupplementalAnnouncementRef = @announcementId and vwPerson.SchoolRef = @schoolId
			Order by 
				vwPerson.Id
				OFFSET @start ROWS FETCH NEXT @count ROWS ONLY
		End
	End