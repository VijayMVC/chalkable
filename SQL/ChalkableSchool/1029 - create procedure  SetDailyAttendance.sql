create procedure [dbo].[spSetDailyAttendance] @personId uniqueidentifier, @date datetime2,  @timeIn int, @timeOut int 
as
SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;
begin transaction
declare @studentDailyAttendanceId uniqueidentifier
set @studentDailyAttendanceId = (select Id from
                                 StudentDailyAttendance sda
                                 where(sda.PersonRef = @personId and sda.Date = @date)
                                )        
if (@studentDailyAttendanceId IS NULL) 
Begin
    set @studentDailyAttendanceId = NewID();
    insert into StudentDailyAttendance (Id,PersonRef, [Date], TimeIn, [TimeOut])    
    values (@studentDailyAttendanceId, @personId, @date, @timeIn,@timeOut)
End                                               
commit

Select * from  StudentDailyAttendance where Id =  @studentDailyAttendanceId;
GO

alter table StudentDailyAttendance
add constraint UQ_PersonRef_Date unique(PersonRef, [Date])
go