-------------------------
-- CREATE ANNOUNCEMENT 
-----------------------------
ALTER procedure [dbo].[spCreateAnnouncement] @schoolId int, @classAnnouncementTypeId int, @personId int, @created datetime2,
											  @expires datetime2, @state int, @gradingStyle int, @classId int
as
begin transaction
declare @callerRole int
select top 1 @callerRole = RoleRef from SchoolPerson where PersonRef = @personId

declare @announcementId int
--declare @markingPeriodClassId int;
--if(@classId is not null)
--	set @markingPeriodClassId = (select Id from MarkingPeriodClass where ClassRef = @classId and MarkingPeriodRef = @markingPeriodId)

declare @isDraft bit = 0

if @state = 0
begin
	select top 1 @announcementId = Id 
	from Announcement
	where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId) and [State] = 0
		  and (@classAnnouncementTypeId is null or ClassAnnouncementTypeRef = @classAnnouncementTypeId)
	order by Created desc

	if @announcementId is null
		select top 1 @announcementId = Id from Announcement
		where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId) and [State] = 0
		order by Created desc

	if @announcementId is not null
	update Announcement set [State] = -1 where Id = @announcementId
end


/*DELETE Attachment*/
delete from AnnouncementAttachment where AnnouncementRef IN (SELECT Id FROM Announcement 
where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [State] = 0)

/*DELETE AnnouncementApplication*/
delete from AnnouncementApplication where AnnouncementRef IN (SELECT Id FROM Announcement
where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [State] = 0)


/*DELETE AnnouncementRecipient*/
delete from AnnouncementRecipient
where AnnouncementRef in (Select id from announcement 
where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [state] = 0)

/*DELETE AnnouncementStandard*/
delete from AnnouncementStandard
where AnnouncementRef in (Select id from announcement 
where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [state] = 0)

/*DELETE Announcement*/
delete from Announcement
where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
AND ClassAnnouncementTypeRef = @classAnnouncementTypeId
AND [State] = 0

/*RESTORE STATE FOR DRAFT*/
if @announcementId is not null
begin
	update Announcement set [State] = 0 where Id = @announcementId
	set @isDraft = 1
end
else begin
	/*INSERT TO ANNOUNCEMENT*/
	insert into Announcement (Created, Expires, ClassAnnouncementTypeRef, [State],GradingStyle,[Order], ClassRef, Dropped, SchoolRef, MayBeDropped, VisibleForStudent)
	values(@created, @expires, @classAnnouncementTypeId, @state, @gradingStyle, 1, @classId, 0, @schoolId, 0, 1);
	set @announcementId = SCOPE_IDENTITY()

	/*GET CONTENT FROM PREV ANNOUNCEMENT*/
	declare @prevContent nvarchar(1024)
	select top 1
	@prevContent = Content from Announcement
	where ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where PersonRef = @personId)
		  and ClassAnnouncementTypeRef = @classAnnouncementTypeId
		  and [State] = 1
  		  and Content is not null
	order by Created desc

	update Announcement set Content = @prevContent where Id = @announcementId
end


commit 

if(@classAnnouncementTypeId is not null and @classId is not null)
begin 
	declare @schoolYearId int
	select @schoolYearId = Id from SchoolYear where @created between StartDate and EndDate
	declare @resT table (res int)
	insert into @resT
	exec [spReorderAnnouncements] @schoolYearId, @classAnnouncementTypeId,  @classId
end
exec spGetAnnouncementDetails @announcementId, @personId, @callerRole, @schoolId

GO


