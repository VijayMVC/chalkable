CREATE Procedure [dbo].[spGetAdminAnnouncementsOrderedByTitle]
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
	@pFrom nvarchar,
	@pTo nvarchar,
	@includeFrom bit,
	@includeTo bit
as

declare 
	@tempLP TLessonPlan

declare 
	@tempAA TAdminAnnouncement

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

exec spInternalSortAdminOrLp  @tempLP, @tempAA, @tempSA, 1, 1, 1, @sort, @pFrom, @pTo, @start, @count, @includeFrom, @includeTo


