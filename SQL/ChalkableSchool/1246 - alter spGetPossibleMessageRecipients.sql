Alter Procedure [dbo].[spGetPossibleMessageRecipients] 
	@callerId int,
	@callerRoleId int,
	@filter1 nvarchar(50), 
	@filter2 nvarchar(50),
	@filter3 nvarchar(50),
	@schoolYearId int,
	@teacherStudentMessagingEnabled bit, 
	@studentMessagingEnabled bit,
	@teacherClassOnly bit,
	@studentClassmatesOnly bit
As

Declare @schoolId int = (Select SchoolRef From SchoolYear where Id = @schoolYearId)

--Declare @personT TPerson
Declare @classT TClass

Declare @personT  table 
(
	Id int,
	RoleRef int,
	SchoolRef int,
	FirstName nvarchar(max),
	LastName nvarchar(max),
	BirthDate datetime2,
	Gender nvarchar(max),
	Salutation nvarchar(max),
	Active bit,
	FirstLoginDate datetime2,
	AddressRef int,
	HasMedicalAlert bit,
	IsAllowedInetAccess bit,
	SpecialInstructions nvarchar(max),
	SpEdStatus nvarchar(max),
	UserId int
)

Insert Into @personT 
Select *
From vwPerson
Where 
	(@filter1 is not null and (FirstName like @filter1 or LastName like @filter1))
	and (@filter2 is null or (FirstName like @filter2 or LastName like @filter2))
	and (@filter3 is null or (FirstName like @filter3 or LastName like @filter3))
	and vwPerson.SchoolRef = @schoolId


-- if caller has no permission send message to student than delete all students 
if (@callerRoleId = 2 and  @teacherStudentMessagingEnabled = 0)
    or (@callerRoleId = 3 and @studentMessagingEnabled = 0)
Begin
 	Delete From @personT Where Id in (Select Student.Id From Student) 
End

--if caller is a teacher
If @callerRoleId = 2
Begin 
	if @teacherStudentMessagingEnabled = 1
	Begin
		 If @teacherClassOnly = 1
				Delete From @personT 
				Where Id in (
								Select p.Id From @personT p
								Where not exists
											(
												Select *
												From ClassPerson
												Join ClassTeacher on ClassTeacher.ClassRef = ClassPerson.ClassRef
												Join Class c on c.Id = ClassTeacher.ClassRef
												Where ClassTeacher.PersonRef = @callerId and c.SchoolYearRef = @schoolYearId 
													  And ClassPerson.PersonRef = p.Id
	   										 ) 
							)
	End
	
	-- getting classes 

	if @teacherClassOnly = 1
		Insert Into @classT
		Select Class.Id, 
			   Class.Name, 
			   Class.[Description],
			   Class.ChalkableDepartmentRef,
			   Class.SchoolYearRef, 
			   Class.PrimaryTeacherRef,
			   Class.MinGradeLevelRef,
			   Class.MaxGradeLevelRef,
			   Class.RoomRef,
			   Class.CourseRef,
			   Class.CourseTypeRef,
			   Class.GradingScaleRef,
			   Class.ClassNumber
		From Class
		Where exists(Select * From ClassTeacher Where ClassTeacher.PersonRef = @callerId And ClassTeacher.ClassRef = Class.Id)
			  And SchoolYearRef = @schoolYearId
			  And (
					(@filter1 is null and @filter2 is null and @filter3 is null) or
					(@filter1 is not null and Name like @filter1 or
					 @filter2 is not null and Name like @filter2 or
					 @filter3 is not null and Name like @filter3)
				  )

	else 
		Insert Into @classT
		Select Class.Id, 
			   Class.Name, 
			   Class.[Description],
			   Class.ChalkableDepartmentRef,
			   Class.SchoolYearRef, 
			   Class.PrimaryTeacherRef,
			   Class.MinGradeLevelRef,
			   Class.MaxGradeLevelRef,
			   Class.RoomRef,
			   Class.CourseRef,
			   Class.CourseTypeRef,
			   Class.GradingScaleRef,
			   Class.ClassNumber
		From Class
		Where 
			  SchoolYearRef = @schoolYearId
			  And (
					(@filter1 is null and @filter2 is null and @filter3 is null) or
					(@filter1 is not null and Name like @filter1 or
					 @filter2 is not null and Name like @filter2 or
					 @filter3 is not null and Name like @filter3)
			   )


End

-- if caller is student
If @callerRoleId = 3 
Begin
		If @studentMessagingEnabled = 1
		Begin
			If @studentClassmatesOnly = 1
			Begin
				Delete From @personT Where Id in 
				(
					Select p.Id From @personT p 
					Where Not exists(
								Select * From ClassPerson cp1
								Join Class c on c.Id = cp1.ClassRef
								Join ClassPerson cp2 on cp2.ClassRef = c.Id
								Where cp1.PersonRef = p.Id and c.SchoolYearRef = @schoolYearId And cp2.PersonRef = @callerId 
							)
				)
			End
		End 
		If @teacherStudentMessagingEnabled = 1
		Begin
			If @teacherClassOnly = 1
				Delete From @personT Where Id in 
				(
					Select p.Id From @personT p 
					Where not exists(
								Select * From ClassTeacher
								Join Class c on c.Id = ClassTeacher.ClassRef
								Join ClassPerson on ClassPerson.ClassRef = c.Id
								Where c.SchoolYearRef = @schoolYearId and ClassPerson.PersonRef = @callerId And ClassTeacher.PersonRef = p.Id
							) 
				)
		End
		Else
			Delete From @personT Where Id in (Select [Staff].Id From [Staff])
End


Select * From @personT

Select * From @classT


GO


