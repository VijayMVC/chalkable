CREATE PROCEDURE [dbo].[spDeleteAnnouncementAttribute]
	@attributeIds TInt32 READONLY

AS

delete aaa from AnnouncementAssignedAttribute aaa where exists (select AttributeTypeId from @attributeIds ai where aaa.AttributeTypeId = ai.Value)
delete aa from AnnouncementAttribute aa where Id in (select Value from @attributeIds)

GO