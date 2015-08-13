--Create Attachment Table 
Create Table Attachment
(
	Id int not null Primary Key IDENTITY(1000000000,1),
	PersonRef int not null constraint FK_Attachment_Person foreign key references Person(Id),
	Name nvarchar(256) not null,
	MimeType nvarchar(256) null,
	UploadedDate datetime2 not null, 
	LastAttachedDate datetime2 not null,
	Uuid nvarchar(max) null,
	SisAttachmentId int null,
	RelativeBlobAddress nvarchar(max) null
)
Go

Create NonClustered Index IX_Attachment_PersonRef
	On Attachment(PersonRef)
Go


Alter Table AnnouncementAttachment
Add AttachmentRef int null constraint FK_AnnouncementAttachment_Attachment foreign key references Attachment(Id)
Go

--------------------------------------------------------
--Added Attachments from AnnouncementAttachements table
--------------------------------------------------------

Alter Table Attachment
Add AnnouncementAttachmentId int
Go

Declare @attachmentIds table (id int, annAttId int)
Declare @districtId nvarchar(max) = cast(DB_NAME() as nvarchar(max))

Insert Into Attachment(PersonRef, Name, MimeType, UploadedDate, LastAttachedDate, Uuid, SisAttachmentId, RelativeBlobAddress, AnnouncementAttachmentId) 
	Output Inserted.Id, Inserted.AnnouncementAttachmentId Into @attachmentIds
Select  
	 PersonRef,
	 Name,
	 null,
	 AttachedDate,
	 AttachedDate,
	 Uuid,
	 SisAttachmentId,
	 Case When SisAttachmentId is null 
		Then cast(Id as nvarchar) + '_' + @districtId
		Else null
	 End,
	 Id	   
From 
	AnnouncementAttachment

Update AnnouncementAttachment
Set AttachmentRef = att.Id
From 
	AnnouncementAttachment 
Join  @attachmentIds att on att.annAttId = AnnouncementAttachment.Id
Go

Alter Table Attachment
Drop Column AnnouncementAttachmentId
Go

Alter Table AnnouncementAttachment
Alter Column AttachmentRef int not null
Go

Create NonClustered Index IX_AnnouncementAttachment_AttachmentRef
	On AnnouncementAttachment(AttachmentRef)
Go


----------------------------------------------------
--Add AttachmentRef to AnnouncementAssignedAttribute
----------------------------------------------------

Alter Table AnnouncementAssignedAttribute
Add AttachmentRef int null constraint FK_AnnouncementAssignedAttribute_Attachment foreign key references Attachment(Id)
Go

	Alter Table Attachment
	Add AnnouncementAssignedAttributeId int
	Go

	Declare @attachmentIds table (id int, aaaId int)
	Declare @districtId nvarchar(max) = cast(DB_NAME() as nvarchar(max))

	Insert Into Attachment(PersonRef, Name, MimeType, UploadedDate, LastAttachedDate, Uuid, SisAttachmentId, RelativeBlobAddress, AnnouncementAssignedAttributeId) 
		Output Inserted.Id, Inserted.AnnouncementAssignedAttributeId Into @attachmentIds
	Select  
		 ann.PrimaryTeacherRef,
		 aaa.SisAttachmentName,
		 aaa.SisAttachmentMimeType,
		 ann.Created,
		 ann.Created,
		 aaa.Uuid,
		 aaa.SisAttributeAttachmentId,
		 Case When aaa.SisAttributeAttachmentId is null 
			  Then cast(aaa.Id as nvarchar) + '_' + @districtId
			  Else null
		 End,
		 aaa.Id	   
	From 
		AnnouncementAssignedAttribute aaa
	Join vwClassAnnouncement ann on ann.Id = aaa.AnnouncementRef 
	Where aaa.SisAttributeAttachmentId is not null and aaa.SisAttributeAttachmentId > 0
	Union 
	Select  
		 ann.PrimaryTeacherRef,
		 aaa.SisAttachmentName,
		 aaa.SisAttachmentMimeType,
		 ann.Created,
		 ann.Created,
		 aaa.Uuid,
		 aaa.SisAttributeAttachmentId,
		 cast(aaa.Id as nvarchar) + '_' + @districtId,
		 aaa.Id	   
	From 
		AnnouncementAssignedAttribute aaa
	Join vwLessonPlan ann on ann.Id = aaa.AnnouncementRef 
	Where aaa.SisAttributeAttachmentId is not null and aaa.SisAttributeAttachmentId > 0


	Update AnnouncementAssignedAttribute
	Set AttachmentRef = att.Id
	From 
		AnnouncementAssignedAttribute 
	Join  @attachmentIds att on att.aaaId = AnnouncementAssignedAttribute.Id

	Go

Alter Table Attachment
Drop Column AnnouncementAssignedAttributeId
Go

Alter Table AnnouncementAssignedAttribute
Add SisActivityId int null 
Go

Update AnnouncementAssignedAttribute
Set SisActivityId = a.SisActivityId
From AnnouncementAssignedAttribute aaa
Join vwClassAnnouncement a on a.Id = aaa.AnnouncementRef
