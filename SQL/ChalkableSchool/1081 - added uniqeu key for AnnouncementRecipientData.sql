delete from AnnouncementRecipientData 
where (select Count(*) 
	   from AnnouncementRecipientData ard
	   where ard.AnnouncementRef = AnnouncementRecipientData.AnnouncementRef
			 and ard.PersonRef = AnnouncementRecipientData.PersonRef
	   group by AnnouncementRef, PersonRef) > 1

go

alter table AnnouncementRecipientData
add constraint QU_AnnouncementRecipientData_AnnouncementRef_PersonRef unique (AnnouncementRef, PersonRef)
go

alter procedure [dbo].[spUpdateAnnouncemetRecipientData] @personId uniqueidentifier, @announcementId uniqueidentifier,
@starred bit, @starredAutomatically int, @currentDate date
as
begin transaction 

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
		set Starred = @starred , StarredAutomatically = @starredAutomatically, LastModifiedDate = @currentDate
		where @id = AnnouncementRecipientData.Id
	end
	else 
	begin
		if(@starredAutomatically is null)
			set @starredAutomatically = 0
	
		insert into AnnouncementRecipientData(Id, AnnouncementRef, PersonRef, Starred, StarredAutomatically, LastModifiedDate)
		values(newid(), @announcementId, @personId, @starred, @starredAutomatically, @currentDate)
	end
commit transaction
GO




