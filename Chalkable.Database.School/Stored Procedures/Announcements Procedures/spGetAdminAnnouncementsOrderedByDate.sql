CREATE Procedure [dbo].[spGetAdminAnnouncementsOrderedByDate]
	@id int, 
	@personId int,
	@roleId int,
	@ownedOnly bit,
	@fromDate DateTime2,
	@toDate DateTime2,
	@start int, 
	@count int,
	@now DateTime2,
	@gradeLevelsIds TInt32 Readonly, 
	@complete bit,
	@studentId int,
	@sort bit,
	@includeFrom bit,
	@includeTo bit
as

declare 
	@tempAA TAdminAnnouncement

declare 
	@tempLP TLessonPlan

declare 
	@tempSA TSupplementalAnnouncement

insert into @tempAA 
	exec spGetAdminAnnouncements
		@id, 
		@personId,
		@roleId,
		@ownedOnly,
		@fromDate,
		@toDate,
		@now,
		@gradeLevelsIds, 
		@complete, 
		@studentId

exec spInternalSortAdminOrLp  @tempLP, @tempAA, @tempSA, 1, 0, 0, @sort, @fromDate, @toDate, @start, @count, @includeFrom, @includeTo
