Alter PROCEDURE [dbo].[spGetPersonDetails] @schoolId int, @personId int, @callerId int, @callerRoleId int
as
--exec spGetPersons @schoolId, @personId, @callerId, null, 0, 1, null, null, null, null,null,null,null, 0, @callerRoleId, null, null, null, 0
select 
	* 
from 
	vwPerson
where
	id = @personId
	and Schoolref = @schoolId

select top 1 a.* from [Address] a
join Person p on p.AddressRef = a.Id
where p.Id = @personId

select * from Phone
where PersonRef = @personId

select *
from StudentSchoolYear 
join GradeLevel on GradeLevel.Id = StudentSchoolYear.GradeLevelRef
where StudentSchoolYear.StudentRef = @personId

GO

Drop Procedure spGetPersons
GO

