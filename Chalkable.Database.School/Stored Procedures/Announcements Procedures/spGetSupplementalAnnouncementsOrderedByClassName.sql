CREATE PROCEDURE [dbo].[spGetSupplementalAnnouncementsOrderedByClassName]
	@id int, 
	@schoolYearId int, 
	@personId int, 
	@classId int, 
	@studentId int,
	@teacherId int,
	@roleId int, 
	@ownedOnly bit,
	@fromDate DateTime2, 
	@toDate DateTime2, 
	@start int, 
	@count int,
	@sort bit,
	@fromClassName nvarchar,
	@toClassName nvarchar,
	@includeFrom bit,
	@includeTo bit,
	@complete bit
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
	@FILTER_BY_CLASS_NAME int = 2,
	@SORT_BY_CLASS_NAME int = 2,
	@SUPPLEMENTAL_ANN_TYPE int = 2

exec spInternalSortAdminOrLp @tempLP, @tempAA, @tempSA, @SUPPLEMENTAL_ANN_TYPE, @FILTER_BY_CLASS_NAME, @SORT_BY_CLASS_NAME, @sort, @fromClassName, @toClassName, @start, @count, @includeFrom, @includeTo



