---Droping old columns from AnnouncementAttachment table 

Alter Table AnnouncementAttachment
Drop Column Name 
Go

Alter Table AnnouncementAttachment
Drop Constraint FK_AnnouncementAttachment_Person
Go

Alter Table AnnouncementAttachment
Drop Column PersonRef
Go


Alter Table AnnouncementAttachment
Drop Column UUid
Go


Alter Table AnnouncementAttachment
Drop Column SisAttachmentId
Go

---Droping old columns from AnnouncementAssignedAttribute table 

Alter Table AnnouncementAssignedAttribute
Drop Column UUid
Go

Alter Table AnnouncementAssignedAttribute
Drop Column SisAttributeAttachmentId
Go

Alter Table AnnouncementAssignedAttribute
Drop Column SisAttachmentName
Go

Alter Table AnnouncementAssignedAttribute
Drop Column SisAttachmentMimeType
Go
