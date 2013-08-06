ALTER procedure [dbo].[spUpdateAnnouncemetRecipientData] @personId uniqueidentifier, @announcementId uniqueidentifier,
														  @starred bit, @starredAutomatically int
as

declare @id uniqueidentifier, @oldStarredAutomatically bit
select @id = anr.id, @oldStarredAutomatically = anr.StarredAutomatically
from AnnouncementRecipientData anr
where anr.AnnouncementRef = @announcementId 
and anr.PersonRef = @personId

if @id is not null
begin 
	 if(@starredAutomatically is null)
		set @starredAutomatically = @oldStarredAutomatically
	 update AnnouncementRecipientData 
	 set Starred = @starred , StarredAutomatically = @starredAutomatically
	 where @id = AnnouncementRecipientData.Id
end
else 
begin
     insert into AnnouncementRecipientData(AnnouncementRef, PersonRef, Starred, StarredAutomatically)
     values (@announcementId, @personId, @starred, @starredAutomatically)
end
GO


