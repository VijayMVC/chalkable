ALTER PROCEDURE [dbo].[spGetStudentCountToAppInstallByClass]
@applicationId uniqueidentifier, @schoolYearId int, @personId int, @roleId int
as
declare @emptyResult bit = 0;
declare @schoolId int = (select SchoolRef from SchoolYear where Id = @schoolYearId)

select
	Class.Id as ClassId,
	Class.Name as ClassName,
	Count(*) as NotInstalledStudentCount
from Class
join ClassPerson on ClassPerson.ClassRef = Class.Id
join Student on Student.Id = ClassPerson.PersonRef
join UserSchool on UserSchool.UserRef = Student.UserId
left join (select * from ApplicationInstall where ApplicationRef = @applicationId and Active = 1) x
on x.PersonRef = ClassPerson.PersonRef and x.SchoolYearRef = Class.SchoolYearRef
where
	@emptyResult = 0 and UserSchool.SchoolRef = @schoolId
	and Class.SchoolYearRef = @schoolYearId
	and (@roleId = 8 or @roleId = 7 or @roleId = 5 or (@roleId = 2 and (Class.PrimaryTeacherRef = @personId or 
																		exists(select * from ClassTeacher ct 
																			   where ct.PersonRef = @personId and ct.ClassRef = Class.Id) 
																		)
													   )
		)
	and x.Id is null
group by
Class.Id, Class.Name
GO


