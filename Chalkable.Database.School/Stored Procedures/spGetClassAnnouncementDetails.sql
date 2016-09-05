



CREATE Procedure [dbo].[spGetClassAnnouncementDetails] @classAnnouncementId int, @callerId int, @callerRole int, @schoolYearId int
As

Declare @classAnn TClassAnnouncementComplex
Declare @allCount int = 1
Declare @isOwner bit = 1
Declare @complete bit =  cast((Select Top 1 Complete from AnnouncementRecipientData where AnnouncementRef = @classAnnouncementId and PersonRef = @callerId) as bit)

If @complete is null 
	Set @complete = 0

If @callerRole = 2
Begin
	insert into @classAnn
	Select 
		vwClassAnnouncement.*, @isOwner, @complete, @allCount
	From vwClassAnnouncement
	Where Id = @classAnnouncementId and SchoolYearRef = @schoolYearId
		  and exists(select * from ClassTeacher where ClassTeacher.PersonRef = @callerId and ClassTeacher.ClassRef = vwClassAnnouncement.ClassRef)   
End
If @callerRole = 3
Begin
	Set @isOwner = 0
	Insert into @classAnn
	Select vwClassAnnouncement.*, @isOwner, @complete, @allCount
	From vwClassAnnouncement
	Where Id = @classAnnouncementId and SchoolYearRef = @schoolYearId
		  and exists(select * from ClassPerson where ClassPerson.PersonRef = @callerId and ClassPerson.ClassRef = vwClassAnnouncement.ClassRef)   
End

Declare @primaryTeacherId int, @classId int
Select @primaryTeacherId = t.PrimaryTeacherRef, @classId = t.ClassRef, @classAnnouncementId = t.Id From @classAnn t


Declare @schoolId int = cast((Select Top 1 SchoolRef From SchoolYear Where Id = @schoolYearId) as int)
Exec spSelectClassAnnouncement @classAnn
Exec spSelectAnnouncementAddionalData @classAnnouncementId, @primaryTeacherId, @callerId, @schoolId

Declare @teacherIds TInt32
Insert Into @teacherIds
Select PersonRef From ClassTeacher Where ClassRef = @classId


Select * From vwAnnouncementAttachment
Where AnnouncementAttachment_AnnouncementRef = @classAnnouncementId
		and (@callerRole = 2 or (@callerRole = 3 and (Attachment_PersonRef = @callerId or exists(select * from @teacherIds t where t.value = Attachment_PersonRef))))