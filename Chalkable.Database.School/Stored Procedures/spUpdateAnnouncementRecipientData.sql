CREATE PROCEDURE [dbo].[spUpdateAnnouncementRecipientData] 
 @announcementId Int, 
 @complete Bit,
 @personId int,
 @roleId int,
 @schoolYearId int,
 @classId int,
 @announcementType int,
 @fromDate datetime2,
 @toDate datetime2,
 @filterByExpiryDate bit
AS

Begin Transaction

--Announcement types
declare @LESSON_PLAN int = 3,
  @ADMIN_ANNOUNCEMENT int = 2;
--Roles ids
declare @TEACHER_ROLE int = 2,
  @STUDENT_ROLE int = 3,
  @DISTRICT_ADMIN_ROLE int = 10;

declare @announcementsToMark table (Id int, Complete bit)
declare @adminAnnouncements TAdminAnnouncement,
  @lessonPlans TLessonPlan,
  @completeStateNeedsUpdate int = 1 - @complete,
  @gradeLev TInt32;

if @announcementType = @ADMIN_ANNOUNCEMENT
Begin
 if @roleId = @STUDENT_ROLE
  insert into @adminAnnouncements
   exec spGetAdminAnnouncements @announcementId, @personId, @roleId, 0, 
    @fromDate, @toDate, null, @gradeLev, @completeStateNeedsUpdate, @personId

 if @roleId = @DISTRICT_ADMIN_ROLE
  insert into @adminAnnouncements
   exec spGetAdminAnnouncements @announcementId, @personId, @roleId, 1, 
    @fromDate, @toDate, null, @gradeLev, @completeStateNeedsUpdate, null
End 
Else If @announcementType = @LESSON_PLAN
Begin
 If @roleId = @TEACHER_ROLE
  insert into @lessonPlans
   exec spGetLessonPlans @announcementId, @schoolYearid, @personId, @classId, @roleId, 
    @personId, null, 1, @fromDate, @toDate, @completeStateNeedsUpdate

 If @roleId = @STUDENT_ROLE
  insert into @lessonPlans
   exec spGetLessonPlans @announcementId, @schoolYearid, @personId, @classId, @roleId, 
    null, @personId, 0, @fromDate, @toDate, @completeStateNeedsUpdate 

	if @filterByExpiryDate = 1
	delete from @lessonPlans
	where @toDate < EndDate
End

declare @announcementsToUpdate table( Id int)
declare @announcementsToInsert table( Id int)

insert into @announcementsToUpdate
 select Id
 from 
  @adminAnnouncements 
  join AnnouncementRecipientData
   on Id = AnnouncementRef 
 Where AnnouncementRecipientData.PersonRef = @personId

 Union

 select id
 from
  @lessonPlans
  join AnnouncementRecipientData
   on Id = AnnouncementRef
 Where AnnouncementRecipientData.PersonRef = @personId

insert into @announcementsToInsert
 select Id
 from 
  @adminAnnouncements 
  left join AnnouncementRecipientData
   on Id = AnnouncementRef and PersonRef = @personId
 Where AnnouncementRecipientData.Complete is null

 Union

 select id
 from
  @lessonPlans
  left join AnnouncementRecipientData
   on Id = AnnouncementRef and PersonRef = @personid
 Where AnnouncementRecipientData.Complete is null

Update AnnouncementRecipientData
Set Complete = @complete 
Where 
 AnnouncementRecipientData.PersonRef = @personId
 And AnnouncementRecipientData.AnnouncementRef in(select * from @announcementsToUpdate)

Insert Into AnnouncementRecipientData
 Select Id, @personId, @complete From @announcementsToInsert

 Commit Transaction

GO