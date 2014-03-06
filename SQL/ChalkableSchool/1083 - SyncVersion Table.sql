Create Table SyncVersion
(
	Id int not null primary key identity,
	TableName nvarchar(256),
	[Version] int
)
GO