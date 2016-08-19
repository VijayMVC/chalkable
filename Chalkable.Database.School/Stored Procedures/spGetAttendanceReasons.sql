

Create Procedure spGetAttendanceReasons
@id int
as
select
[AttendanceReason].[Id] as AttendanceReason_Id,
[AttendanceReason].[Code] as AttendanceReason_Code,
[AttendanceReason].[Name] as AttendanceReason_Name,
[AttendanceReason].[Description] as AttendanceReason_Description,
[AttendanceReason].[Category] as AttendanceReason_Category,
[AttendanceReason].[IsSystem] as AttendanceReason_IsSystem,
[AttendanceReason].[IsActive] as AttendanceReason_IsActive,
[AttendanceLevelReason].[Id] as AttendanceLevelReason_Id,
[AttendanceLevelReason].[Level] as AttendanceLevelReason_Level,
[AttendanceLevelReason].[AttendanceReasonRef] as AttendanceLevelReason_AttendanceReasonRef,
[AttendanceLevelReason].[IsDefault] as AttendanceLevelReason_IsDefault
from
[AttendanceReason]
left join [AttendanceLevelReason] on [AttendanceLevelReason].[AttendanceReasonRef] = [AttendanceReason].[Id]
where
@id is null
or [AttendanceReason].[Id] = @id