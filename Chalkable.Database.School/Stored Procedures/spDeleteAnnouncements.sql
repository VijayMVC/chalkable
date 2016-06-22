CREATE Procedure [dbo].[spDeleteAnnouncements] @announcementIdList TInt32 readonly
as
Begin Transaction

/*DELETE AnnouncementGroups*/
Delete From AnnouncementGroup Where AnnouncementRef in (Select Value From @announcementIdList)

/*DELETE AdminAnnouncement*/
Delete From AdminAnnouncement Where Id in (Select Value From @announcementIdList)

/*Delete LessonPlan*/
Delete From LessonPlan Where Id in (Select Value From @announcementIdList)

/*Delete ClassAnnouncement*/
Delete From ClassAnnouncement Where Id in (Select Value From @announcementIdList)

/*Delete SupplementalAnnouncementRecipient*/
Delete From SupplementalAnnouncementRecipient Where SupplementalAnnouncementRef in (Select Value From @announcementIdList)

/*Delete SupplementalAnnouncement*/
Delete From SupplementalAnnouncement Where Id in (Select Value From @announcementIdList)

/*DELETE AnnouncementRecipientData*/
Delete From AnnouncementRecipientData where AnnouncementRef in (select Value from @announcementIdList)

/*DELETE Attachment*/
Delete From AnnouncementAttachment where AnnouncementRef in (select Value from @announcementIdList)
/*DELETE AnnouncementApplication*/

Delete From [Notification]
Where AnnouncementRef in (select Value from @announcementIdList)

/*DELETE ANOUNCEMENTQNA*/
Delete From AnnouncementQnA
Where AnnouncementRef in (select Value from @announcementIdList)

/*DELETE AutoGrade*/
Delete From AutoGrade
Where AnnouncementApplicationRef IN
(SELECT AnnouncementApplication.Id From AnnouncementApplication
Where AnnouncementRef in (Select Value From @announcementIdList))


/*DELETE AnnouncementApplication*/
Delete From AnnouncementApplication
Where AnnouncementRef in (Select Value From @announcementIdList)

/*DELETE AnnouncementStandard*/
Delete From AnnouncementStandard
Where AnnouncementRef in (Select Value From @announcementIdList)

/*DELETE AnnouncementAssignedAttribute*/
Delete from AnnouncementAssignedAttribute
Where AnnouncementRef in (Select Value From @announcementIdList)

/*DELETE Announcement*/
Delete From Announcement Where Id in (Select Value From @announcementIdList)

Commit