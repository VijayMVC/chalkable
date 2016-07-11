CREATE Procedure [dbo].[spGetLessonPlansOrderedByTitle]
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
	@complete bit, 
	@galleryCategoryId int,
	@sort bit,
	@fromTitle nvarchar,
	@toTitle nvarchar,
	@includeFrom bit,
	@includeTo bit
as

declare 
	@tempLP TLessonPlan

declare
	@tempAA TAdminAnnouncement

insert into @tempLP 
	exec spGetLessonPlans 
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
		@complete, 
		@galleryCategoryId 

exec spInternalSortAdminOrLp @tempLP, @tempAA, 0, 1, 1, @sort, @fromTitle, @toTitle, @start, @count, @includeFrom, @includeTo


