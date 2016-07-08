


CREATE procedure [dbo].[spGetClassAnnouncementsBySisActivities]
@personId int,
@sisActivityIds TInt32 ReadOnly
As

Declare @sisActivityIdsT table(Id int)
Declare @classAnnouncement TClassAnnouncementComplex


If Exists(Select * From @sisActivityIds)
Begin
Insert Into
@sisActivityIdsT(Id)
Select
Value from @sisActivityIds
End

Insert @classAnnouncement
Select
vwClassAnnouncement.*,
cast((case when (select count(*) from ClassTeacher where PersonRef = @personId and ClassTeacher.ClassRef = vwClassAnnouncement.ClassRef) >= 1 then 1 else 0 end) as bit) as IsOwner,
cast(0 as bit) as Complete,
cast(0 as int) as AllCount
From vwClassAnnouncement
Where (Exists(Select * From @sisActivityIds) and SisActivityId is not null and SisActivityId in (select Id from @sisActivityIdsT))

exec spSelectClassAnnouncement @classAnnouncement