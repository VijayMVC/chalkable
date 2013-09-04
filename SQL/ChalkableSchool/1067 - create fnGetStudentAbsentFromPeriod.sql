create function fnGetStudentAbentFromPeriod (@periodOrder int, @fromDate datetime2, @toDate datetime2)	
returns table
as
return
(
	select 
		y.PersonId as PersonId,
		y.[Date] as [Date]
	from
		(select *, cast((Sum(x.[Type] * POWER(10, x.RowNumber)) over(partition by x.PersonId, x.[Date]))  as nvarchar(max)) as [Expression]
			from 
				(	
						select cPerson.PersonRef as PersonId,
							   row_number() over(partition by cPerson.PersonRef, ca.[Date] order by p.[Order]) as RowNumber,
							   ca.[Date] as [Date],
							   case when ca.[Type] = 8 then 4
									when ca.[Type] = 1 then 3
									when ca.[Type] = 4 then 2
									else 1 end as [Type]
						from ClassPerson cPerson 
						join ClassAttendance ca on ca.ClassPersonRef = cPerson.Id 
						join ClassPeriod cp on cp.Id = ca.ClassPeriodRef
						join Period p on p.Id = cp.PeriodRef
						where p.[Order] <= @periodOrder and ca.[Date] >= @fromDate and ca.[Date] <= @toDate
						group by cPerson.PersonRef, ca.[Date], p.[Order], ca.[Type]

				) x
		) y
	where SUBSTRING(y.[Expression], 1, 1) = '4' 
	and (CHARINDEX('1', y.[Expression]) <> 0 or CHARINDEX('2', y.[Expression]) <> 0)
	and (CHARINDEX('2', y.[Expression]) = 0 
		 or (CHARINDEX('1', y.[Expression]) < CHARINDEX('2', y.[Expression]) and CHARINDEX('1', y.[Expression]) <> 0)
		)
	group by y.PersonId, y.[Date]
)
go
