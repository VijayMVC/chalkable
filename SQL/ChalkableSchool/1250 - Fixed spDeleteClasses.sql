Alter Procedure [dbo].[spDeleteClasses]
	@classIds TInt32 readonly
as
	declare @announcmentsToDelete TInt32
	Insert into @announcmentsToDelete
		(value)
	Select Id From LessonPlan where ClassRef in (select value from @classIds as ids)
	Union 
	Select Id From ClassAnnouncement where ClassRef in (select value from @classIds as ids)

	Delete AnnouncementStandard where AnnouncementRef in (Select value From @announcmentsToDelete)
	Delete AnnouncementApplication where AnnouncementRef in (Select value From @announcmentsToDelete)
	Delete [Notification] where AnnouncementRef in (Select value From @announcmentsToDelete)
	Delete AnnouncementAttachment where AnnouncementRef in (Select value From @announcmentsToDelete)
	Delete AnnouncementQnA where AnnouncementRef in (Select value From @announcmentsToDelete)
	Delete AnnouncementRecipientData where AnnouncementRef in (Select value From @announcmentsToDelete)
	Delete AnnouncementAssignedAttribute where AnnouncementRef in (Select value From @announcmentsToDelete)
	Delete From AnnouncementApplication where AnnouncementRef in (Select value From @announcmentsToDelete)

	Delete From ClassAnnouncement where ClassRef in (select value from @classIds as ids)
	Delete From LessonPlan where ClassRef in (select value from @classIds as ids)

	Delete From Announcement where Id in (Select value From @announcmentsToDelete)

	Delete From ApplicationInstallActionClasses where ClassRef in (select value from @classIds as ids)
	Delete From Class where Id in (select value from @classIds as ids)
GO


