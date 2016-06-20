

CREATE Procedure [dbo].[spGetAnnouncementRecipientPersons]
@announcementId int,
@callerId int
As
Declare @classId int,
@schoolId int,
@adminId int,
@schoolYearId int
Select Top 1
@classId = ann.ClassRef,
@adminId = ann.AdminRef,
@schoolYearId = ann.SchoolYearRef
From (
Select
LessonPlan.Id,
LessonPlan.ClassRef,
LessonPlan.SchoolYearRef,
null as AdminRef
From
LessonPlan
Where
LessonPlan.Id = @announcementId
Union
Select
ClassAnnouncement.Id, ClassAnnouncement.ClassRef, ClassAnnouncement.SchoolYearRef, null as AdminRef
From
ClassAnnouncement
Where
ClassAnnouncement.Id = @announcementId
Union
Select
AdminAnnouncement.Id, null as ClassRef, null as SchoolYearRef, AdminAnnouncement.AdminRef as AdminRef
From
AdminAnnouncement
Where
AdminAnnouncement.Id = @announcementId
) ann

Select Top 1
@schoolId = SchoolYear.SchoolRef
From
SchoolYear
Where
ID = @schoolYearId

If(@classId is not null)
Begin
Select
*
From
vwPerson
Where
exists(select * from ClassPerson where ClassRef = @classId and PersonRef = vwPerson.Id) and vwPerson.SchoolRef = @schoolId
End
Else
Begin
Select
*
From
vwPerson join StudentGroup
On StudentGroup.StudentRef = vwPerson.Id join AnnouncementGroup
On AnnouncementGroup.GroupRef = StudentGroup.GroupRef and AnnouncementGroup.AnnouncementRef = @announcementId
End