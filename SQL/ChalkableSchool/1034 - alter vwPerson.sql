alter VIEW [dbo].[vwPerson]
AS
SELECT
	Person.Id as Id,
	SchoolPerson.RoleRef as RoleRef,
	SchoolPerson.SchoolRef as SchoolRef,
	Person.FirstName as FirstName,
	Person.LastName as LastName,
	Person.BirthDate as BirthDate,
	Person.Gender as Gender,
	Person.Salutation as Salutation,
	Person.Active as Active,
	Person.FirstLoginDate as FirstLoginDate,
	Person.Email as Email,
	Person.AddressRef as AddressRef,
	Person.HasMedicalAlert as HasMedicalAlert,
	Person.IsAllowedInetAccess as IsAllowedInetAccess,
	Person.SpecialInstructions as SpecialInstructions,
	Person.SpEdStatus as SpEdStatus
	--,
	--GradeLevel.Id as GradeLevel_Id,
	--GradeLevel.Name as GradeLevel_Name,
	--StudentSchoolYear.SchoolYearRef as StudentSchoolYear_SchoolYearRef	
FROM 
	Person
	Join SchoolPerson on Person.Id = SchoolPerson.PersonRef
	--left join StudentSchoolYear on StudentSchoolYear.StudentRef = Person.Id
	--left join GradeLevel on StudentSchoolYear.GradeLevelRef = GradeLevel.Id	
GO


---------------
----GetPersons
---------------

alter PROCEDURE [dbo].[spGetPersons] @schoolId int, @personId int, @callerId int, @roleId int, @start int, @count int, @startFrom nvarchar(255)
	, @teacherId int, @classId int, @filter1 nvarchar(255), @filter2 nvarchar(255), @filter3 nvarchar(255)
	, @gradeLevelIds nvarchar(1024), @sortType int, @callerRoleId int, @markingPeriodId int, @schoolYearId int, @isEnrolled bit

with recompile
as


declare @glIds table 
(
	id int not null
) 
if @gradeLevelIds is not null
begin
	insert into @glIds
	select cast(s as int) from dbo.split(',', @gradeLevelIds)
end

-------Enrollment Status--------------
-- CurrentlyEnrolled = 0,
-- PreviouslyEnrolled = 1
--------------------------------------

declare @enrollmentStatus int;
if @isEnrolled is not null
	if @isEnrolled = 1 set @enrollmentStatus = 0
	else set @enrollmentStatus = 1

declare @callerGradeLevelId int = null;
if(@callerRoleId = 3)
begin
	-- todo : needs currentSchoolYearId for getting right grade level 
	set @callerGradeLevelId = (select  top 1 GradeLevelRef from StudentSchoolYear where StudentRef = @callerId)
end


select COUNT(*) as AllCount from vwPerson
where
	(@personId is null or Id = @personId)
	and (@roleId is null or RoleRef = @roleId)
	and (@startFrom is null or LastName >= @startFrom)
	and (@teacherId is null or 
		Id in (select ClassPerson.PersonRef from ClassPerson 
				join ClassTeacher on ClassPerson.ClassRef = ClassTeacher.ClassRef 
				where ClassTeacher.PersonRef = @teacherId 
					  and (@markingPeriodId is null or ClassPerson.MarkingPeriodRef = @markingPeriodId)
					  and (@isEnrolled is null or ClassPerson.IsEnrolled = @isEnrolled)
							))
	and (@classId is null or ((@roleId is null or @roleId = 3) and Id in (select PersonRef from ClassPerson 
																	      where ClassPerson.ClassRef = @classId
																			    and (@markingPeriodId is null or ClassPerson.MarkingPeriodRef = @markingPeriodId)
																				and (@isEnrolled is null or ClassPerson.IsEnrolled = @isEnrolled)
																		 )
							  )
						  or ((@roleId is null or @roleId = 2) and Id in (select PersonRef from ClassTeacher where ClassRef = @classId))
		)
	and (@callerRoleId = 1 or (vwPerson.SchoolRef = @schoolId and (@callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 or @callerRoleId = 2 or @callerRoleId = 9
			or(@callerRoleId = 3 and (Id = @callerId   or (RoleRef = 2 or RoleRef = 5 or RoleRef = 7 or RoleRef = 8 
											   or (RoleRef = 3 and exists(select * from StudentSchoolYear
																		  where StudentRef = vwPerson.Id and GradeLevelRef = @callerGradeLevelId and (@schoolYearId is null or SchoolYearRef = @schoolYearId)
																		  and (@enrollmentStatus is null or EnrollmentStatus = @enrollmentStatus)))))
			   )
			or(@callerRoleId = 6 and (Id = @callerId or RoleRef = 3))
		)))	
	and (@filter1 is null or FirstName like @filter1 or LastName like @filter1
		 or FirstName like @filter2 or LastName like @filter2
		 or FirstName like @filter3 or LastName like @filter3)
	and (
			(vwPerson.RoleRef = 3 and exists(select * from StudentSchoolYear ssy 
												where ssy.StudentRef = vwPerson.Id  
													and (@schoolYearId is null or SchoolYearRef = @schoolYearId)
													and (@gradeLevelIds is null or ssy.GradeLevelRef in (select gl.id from @glIds gl))
													and (@enrollmentStatus is null or EnrollmentStatus = @enrollmentStatus)
												)
			)
			or (vwPerson.RoleRef = 2 and
				(@gradeLevelIds is null or exists (select * from Class 
												   join ClassTeacher ct on ct.ClassRef = Class.Id
												   where ct.PersonRef = vwPerson.Id and Class.GradeLevelRef in (select id from @glIds))
				))
		)

-- Sort Type
-- 0 : by FisrtName
-- 1 : by LastName
--------------------			
					
select vwPerson.*
from vwPerson
where
	(@personId is null or  Id = @personId)
	and (@roleId is null or RoleRef = @roleId)
	and (@startFrom is null or LastName >= @startFrom)
	and (@teacherId is null or 
		Id in (select ClassPerson.PersonRef from ClassPerson 
			   join ClassTeacher on ClassPerson.ClassRef = ClassTeacher.ClassRef 
			   where ClassTeacher.PersonRef = @teacherId and (@markingPeriodId is null or ClassPerson.MarkingPeriodRef = @markingPeriodId) 
				    and (@isEnrolled is null or ClassPerson.IsEnrolled = @isEnrolled)))

	and (@classId is null or ((@roleId is null or @roleId = 3) and Id in (select PersonRef from ClassPerson 
																		 where ClassPerson.ClassRef = @classId 
																		 and (@markingPeriodId is null or ClassPerson.MarkingPeriodRef = @markingPeriodId)
																		 and (@isEnrolled is null or ClassPerson.IsEnrolled = @isEnrolled)
																		 ))
						  or ((@roleId is null or @roleId = 2) and Id in (select PersonRef from ClassTeacher where ClassRef = @classId))
		)
	and (@callerRoleId = 1 or (vwPerson.SchoolRef = @schoolId and (@callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 or @callerRoleId = 2 or @callerRoleId = 9
			or(@callerRoleId = 3 and (Id = @callerId 
											or (RoleRef = 2 or RoleRef = 5 or RoleRef = 7 or RoleRef = 8 
												or (RoleRef = 3 and exists(select * from StudentSchoolYear
																		  where StudentRef = vwPerson.Id and GradeLevelRef = @callerGradeLevelId 
																				 and (@schoolYearId is null or SchoolYearRef = @schoolYearId)
																				 and (@enrollmentStatus is null or EnrollmentStatus = @enrollmentStatus)
																		  ))))
				)
			or(@callerRoleId = 6 and (Id = @callerId or RoleRef = 3))
		)))	
	and (@filter1 is null or FirstName like @filter1 or LastName like @filter1
			or FirstName like @filter2 or LastName like @filter2
			or FirstName like @filter3 or LastName like @filter3)
	and (
			(vwPerson.RoleRef = 3 and exists(select * from StudentSchoolYear ssy 
												where ssy.StudentRef = vwPerson.Id  
													and (@schoolYearId is null or SchoolYearRef = @schoolYearId)
													and (@gradeLevelIds is null or ssy.GradeLevelRef in (select gl.id from @glIds gl))
													and (@enrollmentStatus is null or EnrollmentStatus = @enrollmentStatus)
												)
			)
			or (vwPerson.RoleRef = 2 and
				(@gradeLevelIds is null or exists (select * from Class 
												   join ClassTeacher ct on ct.ClassRef = Class.Id
												   where ct.PersonRef = vwPerson.Id and Class.GradeLevelRef in (select id from @glIds))
				))
		)
						
order by  case when @sortType = 0 then FirstName  else LastName end
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY


GO



