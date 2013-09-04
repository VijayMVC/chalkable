create function fnGetStudentsAbsentFromDay(@fromDate datetime2, @toDate datetime2)
returns table
as
return
(
	select 	absentFromDay.PersonId, absentFromDay.[Date]
	from 
		(
			select cp.PersonRef as PersonId,
					Count(ca.Id) as attendanceCount,
					SUM(case when (ca.[Type] != 8) then 1 else 0 end) as isNotAbsent,
					ca.[Date]
			from ClassPerson cp 
			join ClassAttendance ca on ca.ClassPersonRef = cp.Id
			where ca.[Date] >= @fromDate and ca.[Date] <= @toDate
			group by ca.[Date], cp.PersonRef
		) as  absentFromDay
	where absentFromDay.attendanceCount <> 0  and absentFromDay.isNotAbsent = 0
)
go