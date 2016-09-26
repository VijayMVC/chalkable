CREATE Procedure [dbo].[spSearchStudents]
	@start int,
	@count int,
	@classId int,
	@schoolId int = null,
	@gradeLevel int = null,
	@programId int = null,
	@teacherId int,
	@classmatesToid int,
	@schoolYearId int,
	@filter1 nvarchar(50),
	@filter2 nvarchar(50),
	@filter3 nvarchar(50),
	@orderByFirstName bit,
	@markingPeriod int,
	@enrolledOnly bit
As

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

If @classId is null and @teacherId is null and @classmatesToid is null and @markingPeriod is null and @schoolId is null and @gradeLevel is null and @programId is null
Begin
	insert into @t
	exec spSearchStudentBySchoolYearTmp @schoolYearId, @start, @count, @filter1, @filter2, @filter3, @orderByFirstName
End
Else Begin

	Declare @includeWithdraw bit = 1
	If @classId is not null
	Set @includeWithdraw = (Select Top 1 ClassroomOption.IncludeWithdrawnStudents
							From ClassroomOption Where Id = @classId)

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
		Cast (Case When min(StudentSchoolYear.EnrollmentStatus) <> 0 or (max(cast(cs.IsEnrolled As int)) = 0 and @classId is not null) Then 1
			  Else 0 End As bit) As IsWithdrawn,
		Total = count(*) over()
	From
		Student
		join StudentSchoolYear
			on Student.Id = StudentSchoolYear.StudentRef
		left join (select * from ClassPerson join Class on ClassPerson.ClassRef = Class.Id) as cs
			on Student.Id = cs.PersonRef and StudentSchoolYear.SchoolYearRef = cs.SchoolYearRef
		left join MarkingPeriod
			on MarkingPeriod.Id = cs.markingPeriodRef and MarkingPeriod.SchoolYearRef = @schoolYearId
		join StudentSchool
			on Student.Id = StudentSchool.StudentRef
		left join StudentSchoolProgram
			on Student.Id = StudentSchoolProgram.StudentId
	Where
		StudentSchoolYear.SchoolYearRef = @schoolYearId
		and (@teacherId is null
			or cs.ClassRef in (Select ClassTeacher.ClassRef From ClassTeacher Where ClassTeacher.PersonRef = @teacherId))
		and (@classmatesToid is null
			or cs.ClassRef in (Select ClassPerson.ClassRef From ClassPerson Where ClassPerson.PersonRef = @classmatesToid))
		and (@classId is null or cs.ClassRef = @classId)
		and ((@filter1 is null or FirstName like @filter1 or LastName like @filter1)
		and (@filter2 is null or FirstName like @filter2 or LastName like @filter2)
		and (@filter3 is null or FirstName like @filter3 or LastName like @filter3))
		and (@markingPeriod is null or MarkingPeriod.Id = @markingPeriod)
		and (@includeWithdraw = 1 or @includeWithdraw is null or (cs.IsEnrolled = 1 and StudentSchoolYear.EnrollmentStatus = 0))
		and (@schoolId is null or StudentSchool.SchoolRef = @schoolId)
		and (@gradeLevel is null or StudentSchoolYear.GradeLevelRef = @gradeLevel)
		and (@programId is null or StudentSchoolProgram.SchoolProgramId = @programId)
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
		Case When @orderByFirstName = 1 Then FirstName
		Else LastName End
	OFFSET @start ROWS FETCH NEXT @count ROWS ONLY
End

Declare @total int
	Set @total = (Select Top 1 Total f From @t Where @enrolledOnly is null or @enrolledOnly = 0 or IsWithdrawn = 0)

	Select IsNull(@total, 0) As AllCount

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
	Where @enrolledOnly is null or @enrolledOnly = 0 or IsWithdrawn = 0

	declare
		@acadYear int =  (Select top 1 AcadYear from SchoolYear where Id = @schoolYearId)

	declare
		@studentIds TInt32

	insert into @studentIds
	select [Id] From @t	Where @enrolledOnly is null or @enrolledOnly = 0 or IsWithdrawn = 0

	select distinct
		StudentSchoolYear.*,
		School.*,
		GradeLevel.Id as GradeLevel_Id,
		GradeLevel.Name as GradeLevel_Name,
		GradeLevel.[Description] as GradeLevel_Description,
		GradeLevel.Number as GradeLevel_Number
	from 
		StudentSchoolYear
		join SchoolYear
			on SchoolYear.Id = StudentSchoolYear.SchoolYearRef
		join School 
			on School.Id = SchoolYear.SchoolRef
		join GradeLevel 
			on GradeLevel.Id = StudentSchoolYear.GradeLevelRef
	where
		SchoolYear.AcadYear = @acadYear
		and
		(@enrolledOnly is null or @enrolledOnly = 0 or StudentSchoolYear.EnrollmentStatus = 0)
		and
		StudentSchoolYear.StudentRef in (select Value from @studentIds)
		and 
		exists (Select * From StudentSchool Where StudentRef = StudentSchoolYear.StudentRef and SchoolRef = School.Id)