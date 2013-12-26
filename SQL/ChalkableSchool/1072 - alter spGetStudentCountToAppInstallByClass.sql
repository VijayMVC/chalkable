alter procedure [dbo].[spGetStudentCountToAppInstallByClass]
	@applicationId uniqueidentifier, @schoolYearId uniqueidentifier, @personId uniqueidentifier, @roleId int
as

declare @emptyResult bit = 0;



select
	Class.Id as ClassId,
	Class.Name as ClassName,
	Count(*) as NotInstalledStudentCount
from 
	Class
	join ClassPerson on ClassPerson.ClassRef = Class.Id
	left join (select * from ApplicationInstall where ApplicationRef = @applicationId and Active = 1) x
		on x.PersonRef = ClassPerson.PersonRef and x.SchoolYearRef = Class.SchoolYearRef
where
	@emptyResult = 0
	and Class.SchoolYearRef = @schoolYearId
	and (
			@roleId = 8 or @roleId = 7 or @roleId = 5 or @roleId = 2 and Class.TeacherRef = @personId
		)
	and x.Id is null
group by 
	Class.Id, Class.Name
GO


