Begin Transaction

Alter Table MarkingPeriod
	Drop FK_MarkingPeriod_School
GO
Alter Table MarkingPeriod
	Drop Column SchoolRef
GO
Alter Table MarkingPeriodClass
	Drop FK_MarkingPeriodClass_School
GO
Alter Table MarkingPeriodClass
	Drop Column SchoolRef
GO
Alter Table ClassPerson
	Drop FK_ClassPerson_School
GO
Alter Table ClassPerson
	Drop Column SchoolRef
GO

Commit
