CREATE Procedure [dbo].[spDeleteClasses]
@classIds TInt32 readonly
as

Delete From ClassAnnouncement where ClassRef in (select value from @classIds as ids)
Delete From LessonPlan where ClassRef in (select value from @classIds as ids)

Declare @annToDelete Table (id int)
Insert Into 
	@annToDelete
	Select
		Announcement.Id
	From
		Announcement
		left join ClassAnnouncement on Announcement.Id = ClassAnnouncement.Id
		left join LessonPlan on Announcement.Id = LessonPlan.Id
		left join AdminAnnouncement on Announcement.Id = AdminAnnouncement.Id
	Where
		ClassAnnouncement.id is null
		and LessonPlan.id is null
		and AdminAnnouncement.id is null



Delete From AnnouncementStandard where AnnouncementRef in (select id from @annToDelete)
Delete From AnnouncementApplication where AnnouncementRef in (select id from @annToDelete)
Delete From [Notification] where AnnouncementRef in (select id from @annToDelete)
Delete From AnnouncementAttachment where AnnouncementRef in (select id from @annToDelete)
Delete From AnnouncementQnA where AnnouncementRef in (select id from @annToDelete)
Delete From AnnouncementRecipientData where AnnouncementRef in (select id from @annToDelete)
Delete From AnnouncementAssignedAttribute where AnnouncementRef in (select id from @annToDelete)
Delete From AnnouncementApplication where AnnouncementRef in (select id from @annToDelete)


Delete From Announcement where Id in (select id from @annToDelete)


Delete From ApplicationInstallActionClasses where ClassRef in (select value from @classIds as ids)
Delete From Class where Id in (select value from @classIds as ids)