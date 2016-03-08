ALTER PROCEDURE [dbo].[spGetAnnouncementRecipientPersons] @announcementId int, @callerId int 
as
declare @classId int, @schoolId int, @schoolYearId int
select top 1 @classId = Announcement.ClassRef,
			 @schoolId = Announcement.SchoolRef, 
			 @schoolYearId = (select top 1 Id from SchoolYear where SchoolRef = Announcement.SchoolRef and Announcement.Expires between StartDate and EndDate)
from Announcement where Announcement.Id = @announcementId

if(@classId is not null)
begin
	select *, SchoolPerson.RoleRef as RoleRef from Person 
	join SchoolPerson on SchoolPerson.PersonRef = Person.Id
	where exists(select * from ClassPerson where ClassRef = @classId and PersonRef = Person.Id)
	and Person.Id <> @callerId and SchoolPerson.SchoolRef = @schoolId
end
else
begin
    select * from vwPerson
	join StudentGroup on StudentGroup.StudentRef = vwPerson.Id
	join AdminAnnouncementRecipient on AdminAnnouncementRecipient.GroupRef = StudentGroup.GroupRef and AdminAnnouncementRecipient.AnnouncementRef = @announcementId	
end
GO


