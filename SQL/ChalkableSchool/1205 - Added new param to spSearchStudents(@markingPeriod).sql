

Alter Procedure [dbo].[spSearchStudents]
	@start int,
	@count int,
	@classId int,
	@teacherId int,
	@classmatesToid int,
	@schoolYearId int,
	@filter nvarchar(50),
	@orderByFirstName bit,
	@markingPeriod int
As

Declare @includeWithdraw bit = 1
If @classId is not null
Set @includeWithdraw = (Select Top 1 
							ClassroomOption.IncludeWithdrawnStudents 
						From 
							ClassroomOption 
						Where	
							Id = @classId
						)

Declare @t Table
(
	[Id] [int] NOT NULL,
	[FirstName] [nvarchar](255) NOT NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[BirthDate] [datetime2](7) NULL,
	[Gender] [nvarchar](255) NULL,
	[HasMedicalAlert] [bit] NOT NULL,
	[IsAllowedInetAccess] [bit] NOT NULL,
	[SpecialInstructions] [varchar](4096) NOT NULL,
	[SpEdStatus] [nvarchar](256) NULL,
	[PhotoModifiedDate] [datetime2](7) NULL,
	[UserId] [int] NOT NULL,
	IsWithdrawn bit,
	Total int
)

Insert Into 
	@t
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
	Cast (Case
			When min(StudentSchoolYear.EnrollmentStatus) <> 0 or max(cast(ClassPerson.IsEnrolled As int)) = 0 Then 1
		  Else 
			0
		  End As bit) As IsWithdrawn,
	Total = count(*) over()
From
	Student
	join StudentSchoolYear 
		on Student.Id = StudentSchoolYear.StudentRef
	left join ClassPerson 
		on Student.Id = ClassPerson.PersonRef
	left join Class 
		on Class.Id = ClassPerson.ClassRef
	left join MarkingPeriod 
		on MarkingPeriod.Id = ClassPerson.markingPeriodRef and 
		   MarkingPeriod.SchoolYearRef = @schoolYearId
		   
Where
StudentSchoolYear.SchoolYearRef = @schoolYearId and 
Class.SchoolYearRef = @schoolYearId and
(@teacherId is null or 
 ClassPerson.ClassRef in (Select 
							ClassTeacher.ClassRef 
						  From 
							ClassTeacher 
						  Where 
							ClassTeacher.PersonRef = @teacherId
						  )
) and
(@classmatesToid is null or 
 ClassPerson.ClassRef in (Select 
							ClassPerson.ClassRef 
						  From 
							ClassPerson 
						  Where 
							ClassPerson.PersonRef = @classmatesToid
						  )
) and
(@classId is null or 
 ClassPerson.ClassRef = @classId
) and
(@filter is null or 
 FirstName like @filter or 
 LastName like @filter
) and
(@includeWithdraw = 1 or 
 @includeWithdraw is null  or 
 ClassPerson.IsEnrolled = 1
) and
(@markingPeriod is null or 
 MarkingPeriod.Id = @markingPeriod)
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
	Student.UserId
Order By
	Case 
		When @orderByFirstName = 1 Then FirstName
	Else 
		LastName
	End
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY

Declare @total int
Set @total = (Select Top 1 
				Total f
			  From @t)
Select 
	isnull(@total, 0) As AllCount

Select
	[Id],
	[FirstName],
	[LastName],
	[BirthDate],
	[Gender],
	[HasMedicalAlert],
	[IsAllowedInetAccess],
	[SpecialInstructions],
	[SpEdStatus],
	[PhotoModifiedDate],
	[UserId],
	IsWithdrawn
From
	@t

GO


