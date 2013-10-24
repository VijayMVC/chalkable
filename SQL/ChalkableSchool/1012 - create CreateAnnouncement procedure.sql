create procedure [dbo].[spCreateAnnouncement] @announcementTypeId int, @personId uniqueidentifier, @created datetime2, 
@expires datetime2, @state int, @gradingStyle int, @classId uniqueidentifier, @markingPeriodId uniqueidentifier
as
begin transaction
declare @announcementId uniqueidentifier
declare @markingPeriodClassId uniqueidentifier;
if(@classId is not null and @markingPeriodId is not null)
	set @markingPeriodClassId = (select Id from MarkingPeriodClass where ClassRef = @classId and MarkingPeriodRef = @markingPeriodId)

declare @isDraft bit
set @isDraft = 0
if @state = 0
begin

	select 
		top 1 @announcementId = Id from Announcement 
	where 
		PersonRef = @personId and [State] = 0
		and (@announcementTypeId is null or AnnouncementTypeRef = @announcementTypeId)
	order by Created desc

	if @announcementId is null
		select top 1 @announcementId = Id from Announcement
		where PersonRef = @personId and [State] = 0
		order by Created desc

	if @announcementId is not null
		update Announcement set [State] = -1 where Id = @announcementId

end

/*DELETE REMINDER*/
delete from AnnouncementReminder where AnnouncementRef IN (SELECT Id FROM Announcement WHERE PersonRef = @personId 
       AND AnnouncementTypeRef = @announcementTypeId 
       AND [State] = 0)

/*DELETE Attachment*/       
delete from AnnouncementAttachment where AnnouncementRef IN (SELECT Id FROM Announcement WHERE PersonRef = @personId 
       AND AnnouncementTypeRef = @announcementTypeId 
       AND [State] = 0)   
       
/*DELETE AnnouncementApplication*/     
delete from AnnouncementApplication where AnnouncementRef IN (SELECT Id FROM Announcement WHERE PersonRef = @personId 
       AND AnnouncementTypeRef = @announcementTypeId 
       AND [State] = 0)  

/*DELETE StudentAnnouncement*/  
delete from StudentAnnouncement
where AnnouncementRef in (Select id from announcement where PersonRef = @personId 
       AND AnnouncementTypeRef = @announcementTypeId 
       AND [state] = 0)

/*DELETE AnnouncementRecipientData*/ 
delete from AnnouncementRecipientData
where AnnouncementRef in (Select id from announcement where PersonRef = @personId 
       AND AnnouncementTypeRef = @announcementTypeId 
       AND [state] = 0)
       
/*DELETE AnnouncementRecipient*/ 
delete from AnnouncementRecipient
where AnnouncementRef in (Select id from announcement where PersonRef = @personId 
       AND AnnouncementTypeRef = @announcementTypeId 
       AND [state] = 0)

   
/*DELETE Announcement*/                  
delete from Announcement where PersonRef = @personId 
       AND AnnouncementTypeRef = @announcementTypeId 
       AND [State] = 0
       
/*RESTORE STATE FOR DRAFT*/              
if @announcementId is not null
begin
	update Announcement set [State] = 0 where Id = @announcementId
	set @isDraft = 1
end
else begin
/*INSERT TO ANNOUNCEMENT*/     
	set @announcementId = newId()        
	insert into Announcement (Id, PersonRef, Created, Expires, AnnouncementTypeRef, [State],GradingStyle,[Order],MarkingPeriodClassRef, Dropped)      
	values(@announcementId, @personId, @created, @expires, @announcementTypeId, @state, @gradingStyle, 1, @markingPeriodClassId, 0); 
end

/*GET CONTENT FROM PREV ANNOUNCEMENT*/              
declare @prevContent nvarchar(1024)
select top 1 
	@prevContent = Content from Announcement
where 
	PersonRef = @personId 
    and AnnouncementTypeRef = @announcementTypeId 
	and [State] = 1
	and Content is not null
order by Created desc
update Announcement set Content = @prevContent where Id = @announcementId

commit
--Select @announcementId;

--declare @roleId int
--select @roleId = RoleRef from Person where Id = @personId
--if @roleId = 2
--begin
--insert into @tmp
--	exec spGetTeacherAnnouncements 
--	@announcementId, @personId, null, null, null, 0, 0, 0, null, null, null, 0, 1, null, 0
--end
--else begin
--insert into @tmp
--	exec spGetAdminAnnouncements 
--	@announcementId, @personId, null, null, null, 0, 0, null, null, null, 0, 1, null, null
--end
	
--update @tmp set IsDraft = @isDraft

--select * from @tmp

exec spGetAnnouncementDetails @announcementId, @personId
GO


