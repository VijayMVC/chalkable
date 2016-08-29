


CREATE Procedure [dbo].[spUpdateMessagingSettings]
	@districtId uniqueidentifier,
	@schoolId uniqueidentifier,
	@studentMessaging bit,
    @studentToClassOnly bit,
    @teacherToStudentMessaging bit,
    @teacherToClassOnly bit
as
Begin Transaction 
	If @districtId is not null
	Begin
		Update 
			School 
		Set
			StudentMessagingEnabled = @studentMessaging, 
			StudentToClassMessagingOnly = @studentToClassOnly, 
			TeacherToStudentMessaginEnabled = @teacherToStudentMessaging, 
			TeacherToClassMessagingOnly = @teacherToClassOnly
		Where 
			DistrictRef = @districtId And IsMessagingDisabled = 0
	End
	If @schoolId is not null
	Begin
		Update 
			School 
		Set
			StudentMessagingEnabled = @studentMessaging, 
			StudentToClassMessagingOnly = @studentToClassOnly, 
			TeacherToStudentMessaginEnabled = @teacherToStudentMessaging, 
			TeacherToClassMessagingOnly = @teacherToClassOnly		
		Where 
			Id = @schoolId
	End
Commit Transaction