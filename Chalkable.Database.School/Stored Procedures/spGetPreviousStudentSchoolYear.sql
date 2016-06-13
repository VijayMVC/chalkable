CREATE PROCEDURE spGetPreviousStudentSchoolYear
	@studentId int
As

Declare @PREV_ENROLLED_STATUS int = 1;

Select SchoolYear.* 
From StudentSchoolYear 
	 join SchoolYear
		 on StudentSchoolYear.SchoolYearRef = SchoolYear.Id
Where StudentRef = @studentId And EnrollmentStatus = @PREV_ENROLLED_STATUS
Order By EndDate Desc