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

exec spInternalSortAdminOrLp @tempLP, @tempAA, @tempSA, 2, 0, 0, @sort, @fromDate, @toDate, @start, @count, @includeFrom, @includeTo