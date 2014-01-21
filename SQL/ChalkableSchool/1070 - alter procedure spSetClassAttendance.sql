alter procedure [dbo].[spSetClassAttendance] @classPeriodId uniqueidentifier, @date datetime2, @type int, @attendanceReasonId uniqueidentifier
								,@lastModified datetime2, @description nvarchar(1024), @sisId int, @classPersonsIds nvarchar(max)
as

declare @cpIds table 
(
	id uniqueidentifier not null
) 
if @classPersonsIds is not null
begin
	insert into @cpIds
	select cast(s as uniqueidentifier) from dbo.split(',', @classPersonsIds)
end

update ClassAttendance
set [Type] = @type, AttendanceReasonRef = @attendanceReasonId,
	LastModified = @lastModified, [Description] = @description, SisId = @sisId 
from @cpIds cp
where ClassAttendance.ClassPersonRef = cp.id and [Date] = @date and ClassPeriodRef = @classPeriodId

insert into ClassAttendance(Id, ClassPeriodRef, ClassPersonRef, [Date], [Type], AttendanceReasonRef, LastModified, [Description], SisId)
select NEWID(), @classPeriodId, cp.id, @date, @type, @attendanceReasonId, @lastModified, @description, @sisId
from @cpIds cp
where cp.id not in (select ClassPersonRef from ClassAttendance
				    where ClassAttendance.Id is not null 
						  and ClassAttendance.[Date] = @date
						  and ClassAttendance.ClassPeriodRef = @classPeriodId)


select * from ClassAttendance
where [Date] = @date and ClassPeriodRef = @classPeriodId and ClassPersonRef in (select id from @cpIds)
GO


