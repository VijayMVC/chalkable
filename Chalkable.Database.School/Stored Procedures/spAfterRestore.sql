Create Procedure spAfterRestore
as
declare @chalkableTables table
(name nvarchar(2048),
ord int)

insert into @chalkableTables
(name, ord)
values
('PersonSetting', 27),
('Notification', 26),
('AutoGrade', 25),
('PracticeGrade', 24),
('AnnouncementCommentAttachment', 23),
('AnnouncementComment', 22),
('StudentAnnouncementApplicationMeta', 21),
('AnnouncementApplication', 20),
('AnnouncementAttachment', 19),
('Attachment', 18),
('AnnouncementQnA', 17),
('PrivateMessageRecipient', 16),
('PrivateMessage', 15),
('LPGalleryCategory', 14),

('AnnouncementAssignedAttribute', 13),
('AnnouncementGroup', 12),
('AnnouncementRecipientData', 11),
('AnnouncementStandard', 10),
('StudentGroup', 9),
('Group', 8),
('SupplementalAnnouncementRecipient', 7),
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