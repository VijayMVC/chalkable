


CREATE Procedure [dbo].[spGetGroupExplorerData] @groupId int, @ownerId int, @currentDate datetime2
as

declare @schoolT table (Id int, Name nvarchar(max), IsActive bit, IsPrivate bit, IsChalkableEnabled bit, SchoolYearRef int)
insert into @schoolT
select * from
(
select School.Id, School.Name, School.IsActive, School.IsPrivate, School.IsChalkableEnabled,
(select top 1 Id from SchoolYear
where SchoolYear.SchoolRef = School.Id and StartDate <= @currentDate and ArchiveDate is null
order by StartDate desc) as SchoolYearId
from School
where School.IsChalkableEnabled = 1
) school
where school.SchoolYearId is not null and school.SchoolYearId in (select SchoolYearRef from StudentSchoolYear where StudentSchoolYear.EnrollmentStatus = 0)

select * from [Group]
where OwnerRef = @ownerId and Id = @groupId

select * from @schoolT

select * from GradeLevel

select
@groupId as GroupRef,
data.*,
sum(data.StudentsInGradeLevel) over(PARTITION BY data.SchoolRef order by data.SchoolRef) as StudentsInSchool,
sum(data.StudentsGroupInGradeLevel) over(PARTITION BY data.SchoolRef order by data.SchoolRef) as StudentsGroupInSchool
from
(
select
s.id as SchoolRef,
StudentSchoolYear.SchoolYearRef,
StudentSchoolYear.GradeLevelRef,
Count(*) as StudentsInGradeLevel,
Sum(case when sg.GroupRef is not null then 1 else 0 end) as StudentsGroupInGradeLevel
from StudentSchoolYear
join @schoolT s on s.schoolYearRef = StudentSchoolYear.SchoolYearRef
left join (select * from StudentGroup where GroupRef = @groupId) sg on sg.StudentRef = StudentSchoolYear.StudentRef
where StudentSchoolYear.EnrollmentStatus = 0
group by s.id, StudentSchoolYear.SchoolYearRef, StudentSchoolYear.GradeLevelRef
) data