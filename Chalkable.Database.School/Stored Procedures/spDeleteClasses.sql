CREATE Procedure [dbo].[spDeleteClasses]
@classIds TInt32 readonly
as
Delete AnnouncementStandard where AnnouncementRef in (Select Id From ClassAnnouncement where ClassRef in (select value from @classIds as ids))
Delete AnnouncementApplication where AnnouncementRef in (Select Id From ClassAnnouncement where ClassRef in (select value from @classIds as ids))
Delete [Notification] where AnnouncementRef in (Select Id From ClassAnnouncement where ClassRef in (select value from @classIds as ids))
Delete AnnouncementAttachment where AnnouncementRef in (Select Id From ClassAnnouncement where ClassRef in (select value from @classIds as ids))
Delete AnnouncementQnA where AnnouncementRef in (Select Id From ClassAnnouncement where ClassRef in (select value from @classIds as ids))
Delete AnnouncementRecipientData where AnnouncementRef in (Select Id From ClassAnnouncement where ClassRef in (select value from @classIds as ids))
Delete AnnouncementAssignedAttribute where AnnouncementRef in (Select Id From ClassAnnouncement where ClassRef in (select value from @classIds as ids))
Delete From AnnouncementApplication where AnnouncementRef in (Select Id From ClassAnnouncement where ClassRef in (select value from @classIds as ids))

Delete From ClassAnnouncement where ClassRef in (select value from @classIds as ids)

Delete AnnouncementStandard where AnnouncementRef in (Select Id From LessonPlan where ClassRef in (select value from @classIds as ids))
Delete AnnouncementApplication where AnnouncementRef in (Select Id From LessonPlan where ClassRef in (select value from @classIds as ids))
Delete [Notification] where AnnouncementRef in (Select Id From LessonPlan where ClassRef in (select value from @classIds as ids))
Delete AnnouncementAttachment where AnnouncementRef in (Select Id From LessonPlan where ClassRef in (select value from @classIds as ids))
Delete AnnouncementQnA where AnnouncementRef in (Select Id From LessonPlan where ClassRef in (select value from @classIds as ids))
Delete AnnouncementRecipientData where AnnouncementRef in (Select Id From LessonPlan where ClassRef in (select value from @classIds as ids))
Delete AnnouncementAssignedAttribute where AnnouncementRef in (Select Id From LessonPlan where ClassRef in (select value from @classIds as ids))
Delete From AnnouncementApplication where AnnouncementRef in (Select Id From LessonPlan where ClassRef in (select value from @classIds as ids))

Delete From LessonPlan where ClassRef in (select value from @classIds as ids)

Delete From Announcement where Id in
(
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
)

Delete From ApplicationInstallActionClasses where ClassRef in (select value from @classIds as ids)
Delete From Class where Id in (select value from @classIds as ids)