Create Table PrivateMessageRecipient
(
	PrivateMessageRef int not null constraint FK_PrivateMessageRecipient_PrivateMessage foreign key references PrivateMessage(Id),
	RecipientRef int not null constraint FK_PrivateMessageRecipient_Person foreign key references Person(Id),
	RecipientClassRef int null constraint FK_PrivateMessageRecipient_Class foreign key references Class(Id),
	[Read] bit not null,
	DeletedByRecipient bit not null
)
Go

Alter Table PrivateMessageRecipient
Add constraint PK_PrivateMessageRecipient Primary Key (PrivateMessageRef, RecipientRef)
Go


Insert into PrivateMessageRecipient
Select Id, ToPersonRef, null, [Read], DeletedByRecipient
From PrivateMessage



Alter Table PrivateMessage
Drop constraint FK_PrivateMessage_ToPerson
Go
Alter Table PrivateMessage
Drop Column ToPersonRef 
Go

Alter Table PrivateMessage
Drop Column [Read]
Go

Alter Table PrivateMessage
Drop Column DeletedByRecipient
Go

