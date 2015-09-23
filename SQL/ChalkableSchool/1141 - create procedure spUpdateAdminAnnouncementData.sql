create procedure spUpdateAdminAnnouncementData @announcementId int, @personId int, @complete bit
as
begin transaction 
declare @currentComplete bit = (select top 1 Complete from AdminAnnouncementData 
								where AnnouncementRef = @announcementId and PersonRef = @personId)

if @currentComplete is null
begin
	insert into AdminAnnouncementData(AnnouncementRef, PersonRef, Complete)
	values (@announcementId, @personId, @complete) 
end
else if @currentComplete <> @complete
begin
	update AdminAnnouncementData
	set Complete = @complete
	where AnnouncementRef = @announcementId and PersonRef = @personId
end
commit transaction 
Go