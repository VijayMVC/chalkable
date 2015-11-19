Alter Procedure [dbo].[spGetTeacherClasses]
	@schoolYearId int,
	@personId int,
	@markingPeriodId int
As

declare @class TClassDetails

Insert Into 
	@class
Select 
	vwClass.*, 0 as Class_StudentsCount
From 
	vwClass
Where
	Class_SchoolYearRef = @schoolYearId
	and exists(select * from ClassTeacher where ClassRef = Class_Id and PersonRef = @personId)
	and (@markingPeriodId is null or exists(select * from MarkingPeriodClass where ClassRef = Class_Id and MarkingPeriodRef = @markingPeriodId))

Update 
	cc
Set 
	Class_StudentsCount = (select count(*) from ClassPerson where ClassRef = cc.Class_Id)
From 
	@class cc

Exec spSelectClassDetails @class

GO
