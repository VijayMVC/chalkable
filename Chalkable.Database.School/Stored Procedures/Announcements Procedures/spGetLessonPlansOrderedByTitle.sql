﻿CREATE Procedure [dbo].[spGetLessonPlansOrderedByTitle]
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

declare 
	@tempSA TSupplementalAnnouncement

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
		@complete

exec spInternalSortAdminOrLp @tempLP, @tempAA, @tempSA, 0, 1, 1, @sort, @fromTitle, @toTitle, @start, @count, @includeFrom, @includeTo


