create view vwFinalGrade
as
select FinalGrade.Id as FinalGrade_Id,
	   FinalGrade.[Status] as FinalGrade_Status,
	   FinalGrade.GradingStyle as FinalGrade_GradingStyle,
	   FinalGrade.ParticipationPercent as FinalGrade_ParticipationPercent,
	   FinalGrade.Attendance as FinalGrade_Attendance,
	   FinalGrade.Discipline as FinalGrade_Discipline,
	   FinalGrade.DropLowestAttendance as FinalGrade_DropLowestAttendance,
	   FinalGrade.DropLowestDiscipline as FinalGrade_DropLowestDiscipline,
	   MarkingPeriodClass.MarkingPeriodRef as MarkingPeriodClass_MarkingPeriodRef,
	   vwClass.*
from FinalGrade 
join MarkingPeriodClass on MarkingPeriodClass.Id = FinalGrade.Id
join vwClass on vwClass.Class_Id = MarkingPeriodClass.ClassRef
go


create view vwStudentFinalGrade
as
select  
	 StudentFinalGrade.Id as StudentFinalGrade_Id,
	 StudentFinalGrade.ClassPersonRef as StudentFinalGrade_ClassPersonRef,
	 StudentFinalGrade.FinalGradeRef as StudentFinalGrade_FinalGradeRef,
	 StudentFinalGrade.AdminGrade as StudentFinalGrade_AdminGrade,
	 StudentFinalGrade.TeacherGrade as StudentFinalGrade_TeacherGrade,
	 StudentFinalGrade.GradeByAnnouncement as StudentFinalGrade_GradeByAnnouncement,
	 StudentFinalGrade.GradeByAttendance as StudentFinalGrade_GradeByAttendance,
	 StudentFinalGrade.GradeByDiscipline as StudentFinalGrade_GradeByDiscipline,	
	 StudentFinalGrade.GradeByParticipation as StudentFinalGrade_GradeByParticipation,
	 vwPerson.*
from StudentFinalGrade
join ClassPerson on ClassPerson.Id = StudentFinalGrade.ClassPersonRef
join vwPerson on vwPerson.Id = ClassPerson.PersonRef
go

create view vwFinalGradeAnnouncementType
as 
select 
	 FinalGradeAnnouncementType.Id as FinalGradeAnnouncementType_Id,
	 FinalGradeAnnouncementType.AnnouncementTypeRef as FinalGradeAnnouncementType_AnnouncementTypeRef,
	 FinalGradeAnnouncementType.FinalGradeRef as FinalGradeAnnouncementType_FinalGradeRef,
	 FinalGradeAnnouncementType.PercentValue as FinalGradeAnnouncementType_PercentValue,
	 FinalGradeAnnouncementType.GradingStyle as FinalGradeAnnouncementType_GradingStyle,
	 FinalGradeAnnouncementType.DropLowest as FinalGradeAnnouncementType_DropLowest

from FinalGradeAnnouncementType
join AnnouncementType on AnnouncementType.Id = FinalGradeAnnouncementType.AnnouncementTypeRef
go

create procedure spGetFinalGrades @finalGradeId uniqueidentifier, @callerId uniqueidentifier, @callerRoleId int
, @status bit, @markingPeriodId uniqueidentifier, @classId uniqueidentifier, @start int, @count int
as

declare @sourceCount int = 0;

if(@finalgradeId is null)
begin

	select @sourceCount = COUNT(*) 
	from vwFinalGrade 
	where  (@finalGradeId is null or FinalGrade_Id = @finalGradeId)
			and (@markingPeriodId is null or MarkingPeriodClass_MarkingPeriodRef = @markingPeriodId)
			and (@classId is null or Class_Id = @classId)
			and (@status is null or FinalGrade_Status = @status)
			and (@callerRoleId = 1 or @callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 
				or @callerRoleId = 2 or (@callerRoleId = 3 and exists(select * from ClassPerson where ClassRef = Class_Id)))
end

declare @finlaGradeT table(id uniqueidentifier)

insert into @finlaGradeT
select vwFinalGrade.FinalGrade_Id 
from vwFinalGrade 
where (@finalGradeId is null or FinalGrade_Id = @finalGradeId)
	   and (@markingPeriodId is null or MarkingPeriodClass_MarkingPeriodRef = @markingPeriodId)
	   and (@classId is null or Class_Id = @classId)
	   and (@status is null or FinalGrade_Status = @status)
	   and (@callerRoleId = 1 or @callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 
		    or @callerRoleId = 2 or (@callerRoleId = 3 and exists(select * from ClassPerson where ClassRef = Class_Id)))
order by vwFinalGrade.FinalGrade_Id
OFFSET @start ROWS FETCH NEXT @count ROWS ONLY

if(@finalgradeId is not null and  exists(select * from @finlaGradeT))
begin
	set @sourceCount = 1
end
select @sourceCount as SourceCount


select *
from  vwFinalGrade 
where FinalGrade_Id in (select id from @finlaGradeT)


select * from vwFinalGradeAnnouncementType
where FinalGradeAnnouncementType_FinalGradeRef in (select id from @finlaGradeT) 

select * from vwStudentFinalGrade
where StudentFinalGrade_FinalGradeRef in (select id from @finlaGradeT)

go

create procedure spBuildFinalGrade @markingPeriodClassId uniqueidentifier, @callerId uniqueidentifier, @callerRoleId int
as

declare @teacherId uniqueidentifier, @markingPeriodId uniqueidentifier, @classId uniqueidentifier

select @markingPeriodId = MarkingPeriodClass.MarkingPeriodRef
, 	@teacherId = Class.TeacherRef, @classId = Class.Id
from MarkingPeriodClass 
join Class on Class.Id = MarkingPeriodClass.ClassRef
where MarkingPeriodClass.Id = @markingPeriodClassId 
	  and (@callerRoleId = 5 or @callerRoleId = 7 or @callerRoleId = 8 
		  or @callerRoleId = 1 or (@callerRoleId = 2 and Class.TeacherRef = @callerId))

if(@markingPeriodId is not null and @teacherId is not null and @classId is not null)
begin
	delete from FinalGradeAnnouncementType
	where FinalGradeRef = @markingPeriodClassId
	delete from StudentFinalGrade
	where FinalGradeRef = @markingPeriodClassId
	delete from FinalGrade
	where Id = @markingPeriodClassId


	declare @annTypes table(id int)
	insert into @annTypes
	select Id from AnnouncementType
	where Gradable = 1

	declare @anntypesCount int = (select count(*) from @annTypes)
	declare @typePercent int = 100 / @anntypesCount;
	declare @modePercent int = 100 % @anntypesCount;


	declare @previosfinalGrade table
	(
		Id uniqueidentifier,
		[Status] int,
		ParticipationPercent int,
		Discipline int,
		DropLowestDiscipline  bit,
		Attendance int,
		DropLowestAttendance bit,
		GradingStyle int
	)
	insert into @previosfinalGrade
	select top 1 FinalGrade.* from FinalGrade
	join MarkingPeriodClass mpc on mpc.Id = FinalGrade.Id
	join Class c on c.Id = mpc.ClassRef
	where c.TeacherRef = @teacherId and mpc.MarkingPeriodRef = @markingPeriodId

	declare @attendances int = 0, @desciplines int = 0, @gradingStyle int = 0
	declare @fgAnnType table(PercentValue int, AnnouncementTypeId int, GradingStyle int, DropLowest int)

	if(exists(select * from @previosfinalGrade))
	begin
		select @attendances = fg.Attendance, 
				@desciplines = fg.Discipline,
				@modePercent = fg.ParticipationPercent,
				@gradingStyle = fg.GradingStyle
		from @previosfinalGrade fg

		insert into @fgAnnType(PercentValue, AnnouncementTypeId, GradingStyle, DropLowest)
		select fgat.PercentValue, fgat.AnnouncementTypeRef, fgat.GradingStyle, fgat.DropLowest
		from FinalGradeAnnouncementType fgat
		where fgat.FinalGradeRef in (select Id from @previosfinalGrade)

	end
	else
	begin
		insert into @fgAnnType(PercentValue, AnnouncementTypeId, GradingStyle, DropLowest)
		select @typePercent, at.id, @gradingStyle, 0
		from @annTypes at
	end

	declare @finalGradeId uniqueidentifier = @markingPeriodClassId
	insert into FinalGrade(Id, [Status], ParticipationPercent, Attendance, Discipline, GradingStyle, DropLowestAttendance, DropLowestDiscipline)
	values(@markingPeriodClassId, 0, @modePercent, @attendances, @desciplines, @gradingStyle, 0 , 0)


	insert into FinalGradeAnnouncementType(Id, FinalGradeRef, PercentValue, AnnouncementTypeRef, GradingStyle, DropLowest)
	select NEWID(), @finalGradeId, fgAt.PercentValue, 
		   fgAt.AnnouncementTypeId, fgAt.GradingStyle, fgAt.DropLowest
    from @fgAnnType fgAt

	
	insert into StudentFinalGrade(Id, ClassPersonRef, FinalGradeRef, AdminGrade, TeacherGrade
	, GradeByAnnouncement, GradeByAttendance, GradeByDiscipline, GradeByParticipation)
	select NEWID(), cp.Id, @finalGradeId, 0, 0, 0, 0, 0, 0
	from ClassPerson cp
	where cp.ClassRef = @classId 

	exec spGetFinalGrades @finalGradeId, @callerId, @callerRoleId, 0, null, null, 0 , 1
end
go






