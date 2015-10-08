IF OBJECT_ID(N'dbo.PersonSetting', N'U') IS NOT NULL
	drop table PersonSetting

Create Table PersonSetting
(
	[PersonRef] int not null,
	[SchoolYearRef] int not null,
	[Key] nvarchar(256) not null,
	[Value] nvarchar(max) null,

	Constraint PK_PersonSetting_PersonSchoolYearKey Primary Key Clustered
	(PersonRef, SchoolYearRef, [Key]),

	Constraint FK_PersonSetting_Person Foreign Key(PersonRef) 
	References Person (Id),

	Constraint FK_PersonSetting_SchoolYear Foreign Key(SchoolYearRef)
	References SchoolYear(Id)
)

Go