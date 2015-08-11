Create Procedure spDeleteClasses 
	@classIds TInt32 readonly
as	
	Delete AnnouncementStandard where AnnouncementRef in (Select Id From LessonPlan where ClassRef in (select value from @classIds as ids))
	Delete AnnouncementApplication where AnnouncementRef in (Select Id From LessonPlan where ClassRef in (select value from @classIds as ids))
	Delete [Notification] where AnnouncementRef in (Select Id From LessonPlan where ClassRef in (select value from @classIds as ids))
	Delete AnnouncementAttachment where AnnouncementRef in (Select Id From LessonPlan where ClassRef in (select value from @classIds as ids))
	Delete AnnouncementQnA where AnnouncementRef in (Select Id From LessonPlan where ClassRef in (select value from @classIds as ids))
	Delete AnnouncementRecipientData where AnnouncementRef in (Select Id From LessonPlan where ClassRef in (select value from @classIds as ids))
	Delete AnnouncementAssignedAttribute where AnnouncementRef in (Select Id From LessonPlan where ClassRef in (select value from @classIds as ids))	

	Delete Announcement where Id in (Select Id From LessonPlan where ClassRef in (select value from @classIds as ids))
	Delete From LessonPlan where ClassRef in (select value from @classIds as ids)
	Delete From ApplicationInstallActionClasses where ClassRef in (select value from @classIds as ids)
	Delete From Class where Id in (select value from @classIds as ids)


	