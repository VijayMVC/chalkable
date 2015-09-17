Alter Procedure spDeletePersons  
	@personIds TInt32 readonly
as 
	Delete From AnnouncementAttachment 
	where AttachmentRef in (
								Select Attachment.Id From Attachment
								Join @personIds ids on ids.value = Attachment.PersonRef
						   )

	Delete From AnnouncementQnA where AskerRef in (Select value from @personIds as ids)
									or AnswererRef in (Select value from @personIds as ids)
	
	Delete From ApplicationInstall where PersonRef in (Select value from @personIds as ids)
									or OwnerRef in (Select value from @personIds as ids)

	Delete From StudentGroup where StudentRef in (Select value from @personIds as ids)
									or GroupRef in (Select Id From [Group] where OwnerRef in (Select value from @personIds as ids))
	Delete From [Group] where OwnerRef in (Select value from @personIds as ids)
	Delete From AnnouncementRecipientData where PersonRef in (Select value from @personIds as ids)
	
	Delete from ApplicationInstall Where AppInstallActionRef in
		(Select Id From ApplicationInstallAction where PersonRef in (Select value from @personIds as ids)
											or OwnerRef in (Select value from @personIds as ids))
	
	Delete from ApplicationInstallActionClasses Where AppInstallActionRef in
		(Select Id From ApplicationInstallAction where PersonRef in (Select value from @personIds as ids)
											or OwnerRef in (Select value from @personIds as ids))
	
	Delete From Notification where PrivateMessageRef in 
		(Select Id From PrivateMessage where FromPersonRef in (Select value from @personIds as ids)
								or ToPersonRef in (Select value from @personIds as ids))

	Delete From PrivateMessage where FromPersonRef in (Select value from @personIds as ids)
								or ToPersonRef in (Select value from @personIds as ids)
	
	Delete From Notification where PersonRef in (Select value from @personIds as ids)
									or QuestionPersonRef in (Select value from @personIds as ids)

	Delete From LPGalleryCategory where OwnerRef in (Select value from @personIds as ids)
	Delete From AnnouncementGroup where AnnouncementRef in (Select Id From AdminAnnouncement where AdminRef in (Select value from @personIds as ids))


	Delete AnnouncementStandard where AnnouncementRef in (Select Id From AdminAnnouncement where AdminRef in (Select value from @personIds as ids))
	Delete AnnouncementApplication where AnnouncementRef in (Select Id From AdminAnnouncement where AdminRef in (Select value from @personIds as ids))
	Delete [Notification] where AnnouncementRef in (Select Id From AdminAnnouncement where AdminRef in (Select value from @personIds as ids))
	Delete AnnouncementAttachment where AnnouncementRef in (Select Id From AdminAnnouncement where AdminRef in (Select value from @personIds as ids))
	Delete AnnouncementQnA where AnnouncementRef in (Select Id From AdminAnnouncement where AdminRef in (Select value from @personIds as ids))
	Delete AnnouncementRecipientData where AnnouncementRef in (Select Id From AdminAnnouncement where AdminRef in (Select value from @personIds as ids))
	Delete AnnouncementAssignedAttribute where AnnouncementRef in (Select Id From AdminAnnouncement where AdminRef in (Select value from @personIds as ids))
	Delete Announcement where Id in (Select Id From AdminAnnouncement where AdminRef in (Select value from @personIds as ids))
	Delete From AdminAnnouncement where AdminRef in (Select value from @personIds as ids)
	
	Delete From Attachment Where PersonRef in (Select value From @personIds as ids)


	
