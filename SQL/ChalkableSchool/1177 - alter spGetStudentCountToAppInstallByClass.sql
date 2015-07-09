Alter PROCEDURE [dbo].[spGetStudentCountToAppInstallByClass]
@applicationId uniqueidentifier, @schoolYearId int, @personId int, @roleId int
as

declare @classes table
(Id int)

If @roleId <> 10 and @roleId <> 2
begin
	Throw 10003, 'Not supported role', 1;
end
If @roleId = 2
Begin
	Insert Into @classes
	Select 
		Class.Id
	From 
		Class
		join ClassTeacher on Class.Id = ClassTeacher.ClassRef
	Where
		ClassTeacher.PersonRef = @personId		
		and Class.SchoolYearRef = @schoolYearId
End

If @roleId = 10
Begin
	Insert Into @classes
	Select 
		Class.Id		
	From 
		Class
		join SchoolYear on Class.SchoolYearRef = SchoolYear.Id
	Where
		SchoolYear.StartDate <= getDate()
		and SchoolYear.EndDate >= getDate()
		and ArchiveDate is null
End

select
	Class.Id as ClassId,
	Class.Name as ClassName,
	Count(*) as NotInstalledStudentCount
from 
	Class
	join ClassPerson on ClassPerson.ClassRef = Class.Id
	join Student on Student.Id = ClassPerson.PersonRef	
	join @classes c on Class.Id = c.Id
	left join ApplicationInstall on ApplicationInstall.PersonRef = Student.Id 
		and ApplicationInstall.SchoolYearRef = Class.SchoolYearRef
		and ApplicationInstall.ApplicationRef = @applicationid
		and ApplicationInstall.Active = 1
where
	ApplicationInstall.Id is null
group by
	Class.Id, Class.Name

GO


