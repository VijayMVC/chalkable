Create Procedure [dbo].[spSearchStudentBySchoolYear]
@schoolYearId int,
@start int,
@count int,
@filter1 nvarchar(50),
@filter2 nvarchar(50),
@filter3 nvarchar(50),
@orderByFirstName bit

As

Select
Student.Id,
Student.FirstName,
Student.LastName,
Student.BirthDate,
Student.Gender,
Student.HasMedicalAlert,
Student.IsAllowedInetAccess,
Student.SpecialInstructions,
Student.SpEdStatus,
Student.PhotoModifiedDate,
Student.UserId,
Cast (Case When StudentSchoolYear.EnrollmentStatus <> 0 Then 1 Else 0 End As bit) As IsWithdrawn,
Total = count(*) over()
From
Student
join StudentSchoolYear
on Student.Id = StudentSchoolYear.StudentRef
Where
StudentSchoolYear.SchoolYearRef = @schoolYearId
and ((@filter1 is null or FirstName like @filter1 or LastName like @filter1)
and (@filter2 is null or FirstName like @filter2 or LastName like @filter2)
and (@filter3 is null or FirstName like @filter3 or LastName like @filter3))
Group by
Student.Id,
Student.FirstName,
Student.LastName,
Student.BirthDate,
Student.Gender,
Student.HasMedicalAlert,
Student.IsAllowedInetAccess,
Student.SpecialInstructions,
Student.SpEdStatus,
Student.PhotoModifiedDate,
Student.UserId,
StudentSchoolYear.EnrollmentStatus
Order By
Case
When @orderByFirstName = 1 Then FirstName
Else
LastName
End
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY
GO