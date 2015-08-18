Create Procedure spSearchStudents 
	@start int,
	@count int,
	@classId int,
	@teacherId int,
	@schoolYearId int,
	@filter nvarchar(50),
	@orderByFirstName bit
as
select count(*) as AllCount from
(
select
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
from 
	Student
    join ClassPerson on Student.Id = ClassPerson.PersonRef
    join MarkingPeriod on ClassPerson.MarkingPeriodRef = MarkingPeriod.Id
    join StudentSchoolYear on ClassPerson.PersonRef = StudentSchoolYear.StudentRef and StudentSchoolYear.SchoolYearRef = MarkingPeriod.SchoolYearRef
where	
	MarkingPeriod.SchoolYearRef = @schoolYearId	and 
	(@teacherId is null or ClassPerson.ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where ClassTeacher.PersonRef = @teacherId)) and 
	(@classId is null or ClassPerson.ClassRef = @classId) and 
	(@filter is null or FirstName like @filter or LastName like @filter)
group by 
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
) X

select
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
	cast (case		
		when min(StudentSchoolYear.EnrollmentStatus) <> 0 or max(cast(ClassPerson.IsEnrolled as int)) = 0 then 1
		else 0
	end as bit)as IsWithdrawn
from 
	Student
    join ClassPerson on Student.Id = ClassPerson.PersonRef
    join MarkingPeriod on ClassPerson.MarkingPeriodRef = MarkingPeriod.Id
    join StudentSchoolYear on ClassPerson.PersonRef = StudentSchoolYear.StudentRef and StudentSchoolYear.SchoolYearRef = MarkingPeriod.SchoolYearRef
where	
	MarkingPeriod.SchoolYearRef = @schoolYearId	and 
	(@teacherId is null or ClassPerson.ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where ClassTeacher.PersonRef = @teacherId)) and 
	(@classId is null or ClassPerson.ClassRef = @classId) and 
	(@filter is null or FirstName like @filter or LastName like @filter)
group by 
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
order by 
	case when @orderByFirstName = 1 
		then FirstName 
		else LastName
	end
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY 
