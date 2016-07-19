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
	select @currentStudentPosts = Count(*) From AnnouncementComment Where AnnouncementRef = @announcementId and PersonRef = @callerId and Deleted = 0


Declare @commentIds table (Id int)

Insert Into @commentIds
Select AnnouncementComment.Id 
From AnnouncementComment
Join Announcement 
	on Announcement.Id = AnnouncementComment.AnnouncementRef
Where AnnouncementRef = @announcementId 
	  and Announcement.DiscussionEnabled = 1
	  and Deleted = 0
	  and (@TEACHER_ROLE = @roleId or 
			(@STUDENT_ROLE = @roleId and Hidden = 0 and (RequireCommentsEnabled = 0 or @currentStudentPosts > 0) or AnnouncementComment.PersonRef = @callerId)
		  )

Select * From vwAnnouncementComment Where AnnouncementComment_Id in (Select Id From @commentIds)

Select * 
From AnnouncementCommentAttachment
Join Attachment 
	on Attachment.Id = AnnouncementCommentAttachment.AttachmentRef
Where AnnouncementCommentRef In (Select Id From @commentIds)