
Alter View [dbo].[vwAllSchoolsActiveClasses]  
as
	Select
		vwClass.* 
	From 
		vwClass
		join SchoolYear on vwClass.Class_SchoolYearRef = SchoolYear.Id
	Where
		SchoolYear.StartDate <= getDate()
		and SchoolYear.EndDate >=  dateAdd(day, -1, getDate())
		and ArchiveDate is null

GO
