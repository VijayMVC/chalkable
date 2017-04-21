Create Procedure spBeforeRestore
as
begin transaction
declare @chalkableTables table
(name nvarchar(2048))

insert into @chalkableTables
(name)
values
('ReportCardsLogo'),
('AdminAnnouncementStudent'),
('PersonSetting'),
('Notification'),
('AutoGrade'),
('PracticeGrade'),
('AnnouncementCommentAttachment'),
('AnnouncementComment'),
('StudentAnnouncementApplicationMeta'),
('AnnouncementApplication'),
('AnnouncementAttachment'),
('Attachment'),
('AnnouncementQnA'),
('PrivateMessageRecipient'),
('PrivateMessage'),
('LPGalleryCategory'),

('AnnouncementAssignedAttribute'),
('AnnouncementGroup'),
('AnnouncementRecipientData'),
('AnnouncementStandard'),
('StudentGroup'),
('Group'),
('SupplementalAnnouncementRecipient'),
('SupplementalAnnouncement'),
('AdminAnnouncement'),
('ClassAnnouncement'),
('LessonPlan'),
('Announcement')

--Disable all FKs
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
declare @sql nvarchar(3072)
set @sql = 'ALTER TABLE [' + @table + '] NOCHECK CONSTRAINT ALL'

exec sp_executesql @sql

fetch next from TableCursor
into @table
end

CLOSE TableCursor;

--Truncate table data
open TableCursor
fetch next from TableCursor
into @table


while @@FETCH_STATUS = 0
begin
if not exists(select * from @chalkableTables where name = @table)
begin
set @sql = 'Delete from [' + @table	+ ']'
exec sp_executesql @sql
end
fetch next from TableCursor
into @table
end
CLOSE TableCursor;
DEALLOCATE TableCursor;

commit