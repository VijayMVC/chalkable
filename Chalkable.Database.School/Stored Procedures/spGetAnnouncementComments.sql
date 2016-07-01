CREATE PROCEDURE [dbo].[spGetAnnouncementComments]
	@announcementId int,
	@callerId int,
	@roleId int
AS

Declare @TEACHER_ROLE int = 2
Declare @STUDENT_ROLE int = 3
Declare @DISTRICT_ADMIN_ROLE int = 10 
Declare @currentStudentPosts int = 0
If @STUDENT_ROLE = @roleId
	select @currentStudentPosts = Count(*) From AnnouncementComment Where AnnouncementRef = @announcementId and PersonRef = @callerId


Select *
From vwAnnouncementComment
Where AnnouncementComment_AnnouncementRef = @announcementId 
	  and Announcement_DiscussionEnabled = 1
	  and AnnouncementComment_Deleted = 0
	  and (@TEACHER_ROLE = @roleId or 
			(@STUDENT_ROLE = @roleId and AnnouncementComment_Hidden = 0 and (Announcement_RequireCommentsEnabled = 1 or @currentStudentPosts > 0) or AnnouncementComment_PersonRef = @callerId)
		  )
	