create procedure [dbo].[spReorderAnnouncements] @schoolYearId uniqueidentifier, @annType int, 
												@ownerId uniqueidentifier, @classId uniqueidentifier
as
with AnnView as
               (
                select a.Id, Row_Number() over(order by a.Expires, a.[Created]) as [Order]  
                from Announcement a
                join MarkingPeriodClass mpc on mpc.Id = a.MarkingPeriodClassRef
			    join MarkingPeriod mp on mp.Id = mpc.MarkingPeriodRef
                where mp.SchoolYearRef = @schoolYearId and a.AnnouncementTypeRef = @annType 
                      and a.PersonRef = @ownerId and [State] = 1 and mpc.ClassRef = @classId
               )
update Announcement
set [Order] = AnnView.[Order]
from AnnView 
where AnnView.Id = Announcement.Id
select  1
GO


create PROCEDURE [dbo].[spDeleteAnnouncement] @Id uniqueidentifier
AS
delete from AnnouncementReminder where AnnouncementRef = @Id 
/*DELETE Attachment*/       
delete from AnnouncementAttachment where AnnouncementRef = @Id 
/*DELETE AnnouncementApplication*/      
delete from StudentAnnouncement
where AnnouncementRef = @Id
/*DELETE AnnouncementRecipient*/ 
delete from AnnouncementRecipient
where AnnouncementRef = @Id
/*DELETE AnnouncementRecipientData*/ 
delete from AnnouncementRecipientData
where AnnouncementRef = @Id 

/*DELETE NOTIFICATIONS*/
--delete from [Notification]
--where AnnouncementRef = @Id

/*DELETE ANOUNCEMENTQNA*/
delete from AnnouncementQnA
where AnnouncementRef = @Id  

/*DELETE AnnouncementApplication*/     
delete from AnnouncementApplication 
where AnnouncementRef = @Id

declare @annTypeId int, @ownerId uniqueidentifier, @schoolYearId uniqueidentifier, @classId uniqueidentifier

select @annTypeId = a.AnnouncementTypeRef, @ownerId = a.PersonRef, @schoolYearId =  mp.SchoolYearRef, @classId = mpc.ClassRef 
from Announcement a
join MarkingPeriodClass mpc on mpc.Id = a.MarkingPeriodClassRef
join MarkingPeriod mp on mp.Id = mpc.MarkingPeriodRef
where a.Id = @Id  

/*DELETE Announcement*/                  
delete from Announcement where Id = @Id

/*Reordering Process*/
exec spReorderAnnouncements @schoolYearId, @annTypeId, @ownerId, @classId

GO


