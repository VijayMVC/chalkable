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
		vwAllSchoolsActiveClasses.Class_Id		
	From 
		vwAllSchoolsActiveClasses
End

select
	ClassId,
	ClassName,
	Count(*) as NotInstalledStudentCount
from
	(select distinct
		Class.Id as ClassId,
		Class.Name as ClassName,
		ClassPerson.PersonRef	
	from 
		Class
		join @classes c on Class.Id = c.Id
		join ClassPerson on ClassPerson.ClassRef = Class.Id
		join Student on Student.Id = ClassPerson.PersonRef		
		left join (select * from ApplicationInstall
				where ApplicationInstall.ApplicationRef = @applicationid
				and ApplicationInstall.Active = 1
			) a 
			 on a.PersonRef = Student.Id 
			and a.SchoolYearRef = Class.SchoolYearRef
	where
		a.Id is null) X
group by
	ClassId,
	ClassName



GO


