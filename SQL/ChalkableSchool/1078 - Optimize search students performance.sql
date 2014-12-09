Alter Procedure [dbo].[spSearchStudents] 
	@start int,
	@count int,
	@classId int,
	@teacherId int,
	@classmatesToid int,
	@schoolYearId int,
	@filter nvarchar(50),
	@orderByFirstName bit
as

declare @t table
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

Insert into @t
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
	end as bit)as IsWithdrawn,
	Total = count(*) over()
from 
	Student
	join StudentSchoolYear on Student.Id = StudentSchoolYear.StudentRef
    left join ClassPerson on Student.Id = ClassPerson.PersonRef    
	left join MarkingPeriod on MarkingPeriod.Id = ClassPerson.markingPeriodRef and MarkingPeriod.SchoolYearRef = @schoolYearId
where	
	StudentSchoolYear.SchoolYearRef = @schoolYearId and
	(@teacherId is null or ClassPerson.ClassRef in (select ClassTeacher.ClassRef from ClassTeacher where ClassTeacher.PersonRef = @teacherId)) and 
	(@classmatesToid is null or ClassPerson.ClassRef in (select ClassPerson.ClassRef from ClassPerson where ClassPerson.PersonRef = @classmatesToid)) and
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

declare @total int
set @total = (select top 1 Total from @t)
select isnull(@total, 0) as AllCount

select
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


