create function fnGetStudentsAbsentFromDay(@fromDate datetime2, @toDate datetime2, @gradeLevelsIds nvarchar(max))
returns @res table(PersonId uniqueidentifier, [Date] datetime2)
as
begin
	declare @glIds table
	( 
		id uniqueidentifier not null
	) 
	if @gradeLevelsIds is not null
	begin
		insert into @glIds
		select cast(s as uniqueidentifier) from dbo.split(',', @gradeLevelsIds)
	end
	
	--declare @res table(personId uniqueidentifier, [date] datetime2)
	insert into @res
	select 	absentFromDay.PersonId, absentFromDay.[Date]
		from 
			(
				select cp.PersonRef as PersonId,
						Count(ca.Id) as attendanceCount,
						SUM(case when (ca.[Type] != 8) then 1 else 0 end) as isNotAbsent,
						ca.[Date]
				from ClassPerson cp 
				join ClassAttendance ca on ca.ClassPersonRef = cp.Id
				join StudentInfo si on si.Id = cp.PersonRef
				where ca.[Date] >= @fromDate and ca.[Date] <= @toDate
					  and (@gradeLevelsIds is null or si.GradeLevelRef in (select id from @glIds))
				group by ca.[Date], cp.PersonRef
			) as  absentFromDay
		where absentFromDay.attendanceCount <> 0  and absentFromDay.isNotAbsent = 0
	return;
end
go