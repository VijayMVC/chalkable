Create View vwAnnouncementAttachment
As
Select
AnnouncementAttachment.Id as AnnouncementAttachment_Id,
AnnouncementAttachment.AnnouncementRef as AnnouncementAttachment_AnnouncementRef,
AnnouncementAttachment.AttachedDate as AnnouncementAttachment_AttachedDate,
AnnouncementAttachment.[Order] as AnnouncementAttachment_Order,
AnnouncementAttachment.AttachmentRef as AnnouncementAttachment_AttachmentRef,
Attachment.Id as Attachment_Id,
Attachment.PersonRef as Attachment_PersonRef,
Attachment.Name as Attachment_Name,
Attachment.MimeType as Attachment_MimeType,
Attachment.UploadedDate as Attachment_UploadedDate,
Attachment.LastAttachedDate as Attachment_LastAttachedDate,
Attachment.Uuid as Attachment_Uuid,
Attachment.SisAttachmentId as Attachment_SisAttachmentId,
Attachment.RelativeBlobAddress as Attachment_RelativeBlobAddress
From
AnnouncementAttachment
Join Attachment on Attachment.Id = AnnouncementAttachment.AttachmentRef