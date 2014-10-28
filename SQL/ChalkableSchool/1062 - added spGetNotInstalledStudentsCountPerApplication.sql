create procedure spGetNotInstalledStudentsCountPerApplication @staffId int, @classId int, @markingPeriodId int
as
declare @schoolYearId int = (select SchoolYearRef from MarkingPeriod where Id = @markingPeriodId)

declare @personIdsT table(id int)
insert into @personIdsT
select PersonRef from ClassPerson
join StudentSchoolYear ssy on ssy.StudentRef = ClassPerson.PersonRef
where ClassPerson.ClassRef = @classId 
	and ClassPerson.MarkingPeriodRef = @markingPeriodId 
	and ClassPerson.IsEnrolled = 1
	and ssy.EnrollmentStatus = 0
	and ssy.SchoolYearRef = @schoolYearId
group by ClassPerson.PersonRef

declare @appStudentT table(appId uniqueidentifier, personId int)
insert into @appStudentT
select ApplicationRef, PersonRef from ApplicationInstall
where Active = 1 
	and OwnerRef = @staffId 
	and SchoolYearRef = @schoolYearId
	and PersonRef in (select id from @personIdsT)

declare @classStudentCount int = (select count(*) from @personIdsT)
select appSt.appId as ApplicationId, 
	   @classStudentCount - count(appSt.personId) as NotInstalledStudentCount
from @appStudentT appSt
group by appSt.appId
go
