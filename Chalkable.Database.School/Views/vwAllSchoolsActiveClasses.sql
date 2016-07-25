

CREATE View vwAllSchoolsActiveClasses
as
Select
vwClass.*
From
vwClass
join SchoolYear on vwClass.Class_SchoolYearRef = SchoolYear.Id
Where
SchoolYear.StartDate <= getDate()
and SchoolYear.EndDate >= getDate()
and ArchiveDate is null