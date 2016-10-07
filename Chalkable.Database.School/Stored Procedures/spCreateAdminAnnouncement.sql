


CREATE Procedure [dbo].[spCreateAdminAnnouncement] @personId int, @created datetime2, @expires datetime2, @state int
as
begin transaction
--Only districtAdmn can create admin announcement
declare @callerRole int = 10
declare @announcementId int
declare @isDraft bit = 0

if @state = 0
begin
select top 1 @announcementId = Id
from vwAdminAnnouncement
where AdminRef = @personId and [State] = 0
order by Created desc

if @announcementId is not null
update Announcement set [State] = -1 where Id = @announcementId
end

/*Delete AdminAnnouncementData*/

declare @annIdT TInt32
Insert Into @annIdT
Select Id From vwAdminAnnouncement
Where AdminRef = @personId AND [State] = 0

exec spDeleteAnnouncements @annIdT

if @announcementId is not null
begin
update Announcement set [State] = 0 where Id = @announcementId
set @isDraft = 1
end
else begin
/*INSERT TO ANNOUNCEMENT*/
insert into Announcement (Created, Title, Content, [State])
values(@created, null, null, @state)

set @announcementId = SCOPE_IDENTITY()

insert into AdminAnnouncement(Id, AdminRef, Expires)
values(@announcementId, @personId, @expires);


/*GET CONTENT FROM PREV ANNOUNCEMENT*/
--declare @prevContent nvarchar(1024)
--select top 1
--@prevContent = Content from vwAdminAnnouncement
--where AdminRef = @personId and [State] = 1 and Content is not null
--order by Created desc

--update Announcement set Content = @prevContent where Id = @announcementId
end

exec spGetAdminAnnouncementDetails @announcementId, @personId, @callerRole
commit