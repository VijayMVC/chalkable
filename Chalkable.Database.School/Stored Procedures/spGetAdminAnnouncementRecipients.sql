CREATE Procedure [dbo].[spGetAdminAnnouncementRecipients]
	@announcementId int,
	@start int,
	@count int
AS
select
	Student.*,
	null
from
	Student
	join (
		select 
			distinct SG.StudentRef
		from 
			AnnouncementGroup AG
			join StudentGroup SG
				on AG.AnnouncementRef = @announcementId and AG.GroupRef = SG.GroupRef
		) StudentsRef
		on Student.Id = StudentsRef.StudentRef
Order by 
	Student.Id
	OFFSET @start ROWS FETCH NEXT @count ROWS ONLY