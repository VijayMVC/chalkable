CREATE PROCEDURE [dbo].[spGetSupplementalAnnouncements]
	@id int, 
	@schoolYearId int, 
	@personId int, 
	@classId int, 
	@roleId int, 
	@teacherId int,
	@studentId int,
	@ownedOnly bit,
	@fromDate DateTime2, 
	@toDate DateTime2,
	@complete bit,
	@standardId int
AS

declare @TEACHER_ROLE int = 2,
		@STUDENT_ROLE int = 3,
		@DISTRICT_ADMIN_ROLE int = 10;

Select
	vwSupplementalAnnouncement.*,
	cast((case when (select count(*) from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwSupplementalAnnouncement.ClassRef) >= 1 then 1 else 0 end) as bit)  as IsOwner,
	cast((case when annRecipientData.Complete is null then 0 else annRecipientData.Complete end) as bit) as Complete,
	count(vwSupplementalAnnouncement.Id) over() as AllCount
From
	vwSupplementalAnnouncement left join 
	(select * from AnnouncementRecipientData where PersonRef = @personId) annRecipientData 
		on annRecipientData.AnnouncementRef = vwSupplementalAnnouncement.Id
Where
	(@id is not null  or [State] = 1)
	and (@id is null or vwSupplementalAnnouncement.Id = @id)
	and (@classId is null or ClassRef = @classId)
	and (
			(@roleId = @TEACHER_ROLE and (@ownedOnly = 0 or exists(select * from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwSupplementalAnnouncement.ClassRef)))
			or
			(@roleId = @STUDENT_ROLE and exists(select * from SupplementalAnnouncementRecipient where StudentRef = @personId and SupplementalAnnouncementRef = vwSupplementalAnnouncement.Id) and VisibleForStudent = 1)
			or
			 @roleId = @DISTRICT_ADMIN_ROLE --District Id
		)
	and (@teacherId is null or exists(select * from ClassTeacher where PersonRef = @teacherId and ClassTeacher.ClassRef = vwSupplementalAnnouncement.ClassRef))
	and (@studentId is null or exists(select * from SupplementalAnnouncementRecipient where StudentRef = @studentId and SupplementalAnnouncementRef = vwSupplementalAnnouncement.Id))
	and (@fromDate is null or Expires >= @fromDate)
	and (@toDate is null or Expires <= @toDate)
	and (@complete is null or annRecipientData.Complete = @complete or (@complete = 0 and annRecipientData.Complete is null))
	and (@standardId is null or exists(select * from AnnouncementStandard where StandardRef = @standardId and AnnouncementRef = vwSupplementalAnnouncement.Id))
	and (@schoolYearId is null or SchoolYearRef = @schoolYearId)
