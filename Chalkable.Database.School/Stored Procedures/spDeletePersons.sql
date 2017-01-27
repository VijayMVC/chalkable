CREATE Procedure [dbo].[spDeletePersons]
@personIds TInt32 readonly
as
Delete From AnnouncementAttachment
where AttachmentRef in (
Select Attachment.Id From Attachment
Join @personIds ids on ids.value = Attachment.PersonRef
)

Delete From AnnouncementQnA where AskerRef in (Select value from @personIds as ids)
or AnswererRef in (Select value from @personIds as ids)

Delete From StudentGroup where StudentRef in (Select value from @personIds as ids)
or GroupRef in (Select Id From [Group] where OwnerRef in (Select value from @personIds as ids))
Delete From [Group] where OwnerRef in (Select value from @personIds as ids)
Delete From AnnouncementRecipientData where PersonRef in (Select value from @personIds as ids)

Delete From PrivateMessageRecipient where RecipientRef in (Select value from @personIds as ids)

declare @messagesToDelete TInt32;

Insert Into @messagesToDelete
Select Id From PrivateMessage where FromPersonRef in (Select value from @personIds as ids)

Insert Into @messagesToDelete
Select Id From PrivateMessage where not exists(Select * From PrivateMessageRecipient where PrivateMessageRef = PrivateMessage.Id)


Delete From Notification where PrivateMessageRef in
(Select value From @messagesToDelete)

Delete From PrivateMessage where id in (Select value From @messagesToDelete)

Delete From Notification where PersonRef in (Select value from @personIds as ids)
or QuestionPersonRef in (Select value from @personIds as ids)

Delete From LPGalleryCategory where OwnerRef in (Select value from @personIds as ids)
Delete From AnnouncementGroup where AnnouncementRef in (Select Id From AdminAnnouncement where AdminRef in (Select value from @personIds as ids))

Delete AnnouncementComment where AnnouncementRef in (Select Id From AdminAnnouncement where AdminRef in (Select value from @personIds as ids))
Delete AnnouncementStandard where AnnouncementRef in (Select Id From AdminAnnouncement where AdminRef in (Select value from @personIds as ids))
Delete AnnouncementApplication where AnnouncementRef in (Select Id From AdminAnnouncement where AdminRef in (Select value from @personIds as ids))
Delete [Notification] where AnnouncementRef in (Select Id From AdminAnnouncement where AdminRef in (Select value from @personIds as ids))
Delete AnnouncementAttachment where AnnouncementRef in (Select Id From AdminAnnouncement where AdminRef in (Select value from @personIds as ids))
Delete AnnouncementQnA where AnnouncementRef in (Select Id From AdminAnnouncement where AdminRef in (Select value from @personIds as ids))
Delete AnnouncementRecipientData where AnnouncementRef in (Select Id From AdminAnnouncement where AdminRef in (Select value from @personIds as ids))
Delete AnnouncementAssignedAttribute where AnnouncementRef in (Select Id From AdminAnnouncement where AdminRef in (Select value from @personIds as ids))

Declare @announcementIds TInt32;
Insert Into @announcementIds
	Select Id From AdminAnnouncement Where AdminRef in (Select [value] from @personIds as ids)
Delete From AdminAnnouncement where Id in (Select * From @announcementIds)
Delete Announcement where Id in (Select * From @announcementIds)

Delete From Attachment Where PersonRef in (Select value From @personIds as ids)

Delete from Person Where Id in (Select value From @personIds as ids)
GO