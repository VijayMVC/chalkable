Create Procedure spAfterRestore
as
declare @chalkableTables table
(name nvarchar(2048),
ord int)

insert into @chalkableTables
(name, ord)
values
('PersonSetting', 24),
('Notification', 23),
('AutoGrade', 22),
('PracticeGrade', 21),
('AnnouncementComment', 20),
('AnnouncementApplication', 19),
('AnnouncementAttachment', 18),
('Attachment', 17),
('AnnouncementQnA', 16),
('PrivateMessageRecipient', 15),
('PrivateMessage', 14),
('LPGalleryCategory', 13),

('AnnouncementAssignedAttribute', 12),
('AnnouncementGroup', 11),
('AnnouncementRecipientData', 10),
('AnnouncementStandard', 9),
('StudentGroup', 8),
('Group', 7),


('SupplementalAnnouncement', 6),

('AdminAnnouncement', 5),
('ClassAnnouncement', 4),
('LessonPlan', 3),
('Announcement', 2)


if exists(
SELECT
RC.CONSTRAINT_NAME FK_Name
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KF ON RC.CONSTRAINT_NAME = KF.CONSTRAINT_NAME
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KP ON RC.UNIQUE_CONSTRAINT_NAME = KP.CONSTRAINT_NAME
JOIN @chalkableTables ON KF.TABLE_NAME = Name
group by
RC.CONSTRAINT_NAME
having count(*) > 1)
begin

Throw 10000, 'Not implemented for complex FKs yet', 1;
end

declare  ForeignKeyCursor cursor local for
SELECT
KF.TABLE_NAME FK_Table
, KF.COLUMN_NAME FK_Column
, KP.TABLE_NAME PK_Table
, KP.COLUMN_NAME PK_Column
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KF ON RC.CONSTRAINT_NAME = KF.CONSTRAINT_NAME
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KP ON RC.UNIQUE_CONSTRAINT_NAME = KP.CONSTRAINT_NAME
JOIN @chalkableTables ON KF.TABLE_NAME = Name
ORDER BY ord

declare @fkTable nvarchar(2048)
declare @fkColumn nvarchar(2048)
declare @pkColumn nvarchar(2048)
declare @pkTable nvarchar(2048)

open ForeignKeyCursor
fetch next from ForeignKeyCursor
into @fkTable, @fkColumn, @pkTable, @pkColumn

while @@FETCH_STATUS = 0
begin
declare @sql nvarchar(3072) = 'delete from [' + @fkTable + '] where [' + @fkColumn
+ '] IS NOT NULL AND [' + @fkColumn + ']  not in (select [' + @pkColumn + '] from [' + @pkTable + '])'
exec sp_executesql @sql
fetch next from ForeignKeyCursor
into @fkTable, @fkColumn, @pkTable, @pkColumn
end
CLOSE ForeignKeyCursor;
DEALLOCATE ForeignKeyCursor;

--Reenable foreign keys
declare @table nvarchar(2048)
declare  TableCursor cursor local for
SELECT Table_Name FROM
information_schema.tables
where
Table_Type = 'BASE TABLE'


open TableCursor
fetch next from TableCursor
into @table


while @@FETCH_STATUS = 0
begin
set @sql = 'ALTER TABLE [' + @table + '] WITH CHECK CHECK CONSTRAINT ALL'

exec sp_executesql @sql

fetch next from TableCursor
into @table
end

CLOSE TableCursor;
DEALLOCATE TableCursor;