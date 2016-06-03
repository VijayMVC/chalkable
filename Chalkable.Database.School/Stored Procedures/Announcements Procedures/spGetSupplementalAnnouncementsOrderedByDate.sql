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
	@includeTo bit
AS
Declare @gradeLevelsIdsT table(value int);