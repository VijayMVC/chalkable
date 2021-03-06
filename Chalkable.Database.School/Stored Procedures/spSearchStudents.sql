﻿CREATE Procedure [dbo].[spSearchStudents]
	@start int,
	@count int,
	@classId int,
	@schoolIds TInt32 READONLY,
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
	@enrolledOnly bit,
	@callerId int
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

declare @acadYear int =  (Select top 1 AcadYear from SchoolYear where Id = @schoolYearId);

Declare @includeWithdraw bit = 1
If @classId is not null
Set @includeWithdraw = (Select Top 1 ClassroomOption.IncludeWithdrawnStudents From ClassroomOption Where Id = @classId)

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
		join SchoolYear
			on SchoolYear.Id = StudentSchoolYear.SchoolYearRef
		left join (select * from ClassPerson join Class on ClassPerson.ClassRef = Class.Id) as cs
			on Student.Id = cs.PersonRef and StudentSchoolYear.SchoolYearRef = cs.SchoolYearRef
		left join MarkingPeriod
			on MarkingPeriod.Id = cs.markingPeriodRef and MarkingPeriod.SchoolYearRef = @schoolYearId
		join StudentSchool
			on Student.Id = StudentSchool.StudentRef and StudentSchool.SchoolRef = SchoolYear.SchoolRef
		left join StudentSchoolProgram
			on Student.Id = StudentSchoolProgram.StudentId
	Where
		SchoolYear.AcadYear = @acadYear
		and (@teacherId is null
			or cs.ClassRef in (Select ClassTeacher.ClassRef From ClassTeacher Where ClassTeacher.PersonRef = @teacherId))
		and (@classmatesToid is null
			or cs.ClassRef in (Select ClassPerson.ClassRef From ClassPerson Where ClassPerson.PersonRef = @classmatesToid))
		and (@classId is null or cs.ClassRef = @classId)
		and (@markingPeriod is null or MarkingPeriod.Id = @markingPeriod)
		and (@includeWithdraw = 1 or @includeWithdraw is null or (cs.IsEnrolled = 1 and StudentSchoolYear.EnrollmentStatus = 0))
		and (SchoolYear.SchoolRef in (select [Value] from @schoolIds))
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
		IsWithdrawn,
		cast(Case When Exists(select * from ClassPerson join ClassTeacher 
				   On ClassPerson.ClassRef = ClassTeacher.ClassRef 
					  and ClassPerson.PersonRef = [Id] and ClassTeacher.PersonRef = @callerId) 
			Then 1 Else 0 End as bit) As IsMyStudent,
		cast(Case When Exists(select * from ClassPerson studClasses1 join ClassPerson studClasses2
					On studClasses1.ClassRef = studClasses2.ClassRef
						and studClasses1.PersonRef = [Id] and studClasses2.PersonRef = @callerId)
			 Then 1 Else 0 End as bit) as IsClassmate
	From
		@t
	Where (@enrolledOnly is null or @enrolledOnly = 0 or IsWithdrawn = 0)
		and ((@filter1 is null or FirstName like @filter1 or LastName like @filter1)
		and (@filter2 is null or FirstName like @filter2 or LastName like @filter2)
		and (@filter3 is null or FirstName like @filter3 or LastName like @filter3))
	Order By
		Case When @orderByFirstName = 1 Then FirstName
		Else LastName End
	OFFSET @start ROWS FETCH NEXT @count ROWS ONLY

	declare @studentIds TInt32

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
		StudentSchoolYear.StudentRef in (select [Value] from @studentIds)
		and
		(School.Id in (select [Value] from @schoolIds))
		and 
		exists (Select * From StudentSchool Where StudentRef = StudentSchoolYear.StudentRef and SchoolRef = School.Id)
GO