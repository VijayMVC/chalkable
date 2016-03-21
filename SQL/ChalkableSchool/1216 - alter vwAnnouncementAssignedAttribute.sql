Alter View [dbo].[vwAnnouncementAssignedAttribute]
As
Select
	 AnnouncementAssignedAttribute.Id As AnnouncementAssignedAttribute_Id,
	 AnnouncementAssignedAttribute.Name As AnnouncementAssignedAttribute_Name,
	 AnnouncementAssignedAttribute.[Text] As AnnouncementAssignedAttribute_Text,
	 AnnouncementAssignedAttribute.AnnouncementRef As AnnouncementAssignedAttribute_AnnouncementRef,
	 AnnouncementAssignedAttribute.AttributeTypeId As AnnouncementAssignedAttribute_AttributeTypeId,
	 AnnouncementAssignedAttribute.VisibleForStudents As AnnouncementAssignedAttribute_VisibleForStudents,
	 AnnouncementAssignedAttribute.SisActivityAssignedAttributeId As AnnouncementAssignedAttribute_SisActivityAssignedAttributeId,
	 AnnouncementAssignedAttribute.AttachmentRef As AnnouncementAssignedAttribute_AttachmentRef,
	 
	 Attachment.Id As Attachment_Id,
	 Attachment.PersonRef As Attachment_PersonRef,
	 Attachment.Name As Attachment_Name,
	 Attachment.MimeType As Attachment_MimeType,
	 Attachment.UploadedDate As Attachment_UploadedDate,
	 Attachment.LastAttachedDate As Attachment_LastAttachedDate,
	 Attachment.Uuid As Attachment_Uuid,
	 Attachment.SisAttachmentId As Attachment_SisAttachmentId,
	 Attachment.RelativeBlobAddress As Attachment_RelativeBlobAddress
From 
	AnnouncementAssignedAttribute
Left Join Attachment On Attachment.Id = AnnouncementAssignedAttribute.AttachmentRef

GO


