CREATE PROCEDURE [dbo].[spGetSupplementalAnnouncementsOrderedByTitle]
	@id int, 
	@schoolYearId int, 
	@personId int, 
	@studentId int,
	@teacherId int,
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
		@teacherId,
		@studentId,
		@ownedOnly, 
		@fromDate, 
		@toDate,
		@complete

declare 
	@FILTER_BY_TITLE int = 1,
	@SORT_BY_TITLE int = 1,
	@SUPPLEMENTAL_ANN_TYPE int = 2

exec spInternalSortAdminOrLp @tempLP, @tempAA, @tempSA, @SUPPLEMENTAL_ANN_TYPE, @FILTER_BY_TITLE, @SORT_BY_TITLE, @sort, @fromTitle, @toTitle, @start, @count, @includeFrom, @includeTo
