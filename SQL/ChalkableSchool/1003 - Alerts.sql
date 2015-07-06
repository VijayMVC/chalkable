Alter Table Person
	Add HasMedicalAlert bit
GO

Alter Table Person
	Add IsAllowedInetAccess bit
GO

Alter Table Person
	Add SpecialInstructions nvarchar(1024)
GO

Alter Table Person
	Add SpEdStatus nvarchar(256)
GO

Update Person set HasMedicalAlert = 0, SpecialInstructions = '', IsAllowedInetAccess = 0

GO

Alter Table Person
	Alter Column HasMedicalAlert bit not null
GO

Alter Table Person
	Alter Column IsAllowedInetAccess bit not null
GO

Alter Table Person
	Alter Column SpecialInstructions nvarchar(1024) not null
GO