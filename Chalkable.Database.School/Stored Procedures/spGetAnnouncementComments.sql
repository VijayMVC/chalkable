CREATE PROCEDURE [dbo].[spGetAnnouncementComments]
	@announcementId int,
	@callerId int,
	@roleId int
AS

--TODO 
Select *
From AnnouncementComment	
Where AnnouncementRef = @announcementId