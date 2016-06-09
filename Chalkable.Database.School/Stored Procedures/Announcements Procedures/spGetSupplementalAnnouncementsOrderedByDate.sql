CREATE PROCEDURE [dbo].[spGetSupplementalAnnouncementsOrderedByDate]
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
	@includeFrom bit,
	@includeTo bit,
	@complete bit
AS

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

declare 
	@FILTER_BY_DATE int = 0,
	@SORT_BY_DATE int = 0,
	@SUPPLEMENTAL_ANN_TYPE int = 2
	
exec spInternalSortAdminOrLp @tempLP, @tempAA, @tempSA, @SUPPLEMENTAL_ANN_TYPE, @FILTER_BY_DATE, @SORT_BY_DATE, @sort, @fromDate, @toDate, @start, @count, @includeFrom, @includeTo