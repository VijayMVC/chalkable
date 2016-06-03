CREATE PROCEDURE [dbo].[spGetSupplementalAnnouncementsOrderedByTitle]
	@id int, 
	@schoolYearId int, 
	@personId int, 
	@classId int, 
	@roleId int, 
	@ownedOnly bit,
	@fromDate DateTime2, 
	@toDate DateTime2, 
	@start int, 
	@count int,
	@sort bit,
	@fromTitle nvarchar,
	@toTitle nvarchar,
	@includeFrom bit,
	@includeTo bit,
	@complete  bit
as

declare 
	@tempLP TLessonPlan

declare
	@tempAA TAdminAnnouncement

declare 
	@tempSA TSupplementalAnnouncement

insert into @tempSA 
	exec spGetSupplementalAnnouncements 
		@id, 
		@schoolYearId, 
		@personId, 
		@classId,
		@roleId, 
		null,
		null,
		@ownedOnly, 
		@fromDate, 
		@toDate,
		@complete

exec spInternalSortAdminOrLp @tempLP, @tempAA, @tempSA, 2, 1, 1, @sort, @fromTitle, @toTitle, @start, @count, @includeFrom, @includeTo
