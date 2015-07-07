alter Procedure [dbo].[spDeleteAnnouncements] @announcementIdList TIntId readonly
as
Begin Transaction

	/*DELETE AnnouncementGroups*/
	Delete From AnnouncementGroup Where AnnouncementRef in (Select Id From @announcementIdList)

	/*DELETE AdminAnnouncement*/
	Delete From AdminAnnouncement Where Id in (Select Id From @announcementIdList)

	/*Delete LessonPlan*/
	Delete From LessonPlan Where Id in (Select Id From @announcementIdList)

	/*Delete ClassAnnouncement*/
	Delete From ClassAnnouncement Where Id in (Select Id From @announcementIdList)
			
	/*DELETE AnnouncementRecipientData*/
	Delete From AnnouncementRecipientData where AnnouncementRef in (select Id from @announcementIdList)

	/*DELETE Attachment*/
	Delete From AnnouncementAttachment where AnnouncementRef in (select Id from @announcementIdList)
	/*DELETE AnnouncementApplication*/

	Delete From [Notification]
	Where AnnouncementRef in (select Id from @announcementIdList)

	/*DELETE ANOUNCEMENTQNA*/
	Delete From AnnouncementQnA
	Where AnnouncementRef in (select Id from @announcementIdList)

	/*DELETE AutoGrade*/
	Delete From AutoGrade
	Where AnnouncementApplicationRef IN 
		(SELECT AnnouncementApplication.Id From AnnouncementApplication
		 Where AnnouncementRef in (Select Id From @announcementIdList))


	/*DELETE AnnouncementApplication*/
	Delete From AnnouncementApplication
	Where AnnouncementRef in (Select Id From @announcementIdList)

	/*DELETE AnnouncementStandard*/
	Delete From AnnouncementStandard
	Where AnnouncementRef in (Select Id From @announcementIdList)

	/*DELETE AnnouncementAssignedAttribute*/
	Delete from AnnouncementAssignedAttribute
	Where AnnouncementRef in (Select Id From @announcementIdList)

	/*DELETE Announcement*/
	Delete From Announcement Where Id in (Select Id From @announcementIdList)
	
Commit

GO