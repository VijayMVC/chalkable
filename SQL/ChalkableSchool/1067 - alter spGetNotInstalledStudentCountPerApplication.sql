alter procedure [dbo].[spGetNotInstalledStudentsCountPerApplication] @staffId int, @classId int, @markingPeriodId int
as
declare @schoolYearId int = (select SchoolYearRef from MarkingPeriod where Id = @markingPeriodId)

declare @appsIdsT table(id uniqueidentifier)
insert into @appsIdsT
select ApplicationRef from ApplicationInstall
where PersonRef = @staffId and Active = 1 and SchoolYearRef = @schoolYearId

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

declare @classStudentCount int = (select count(*) from @personIdsT)
if @classStudentCount = 0
begin
select id as ApplicationId, 0 as NotInstalledStudentCount from @appsIdsT app
end
else
begin
declare @appStudentT table(appId uniqueidentifier, studentCount int)
insert into @appStudentT
select ApplicationRef, count(PersonRef) from ApplicationInstall
where Active = 1
and ApplicationRef in (select app.id from @appsIdsT app)
and SchoolYearRef = @schoolYearId
and PersonRef in (select id from @personIdsT)
group by ApplicationRef





select app.id as ApplicationId,
(case when appSt.appId is null then @classStudentCount else @classStudentCount - appSt.studentCount end) as NotInstalledStudentCount
from @appsIdsT app
left join @appStudentT appSt on appSt.appId = app.id
end

GO


