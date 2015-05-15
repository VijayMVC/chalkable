IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'VisibleInClassroom' AND Object_ID = Object_ID('dbo.Infraction'))
BEGIN
ALTER TABLE dbo.Infraction
	ADD VisibleInClassroom BIT NOT NULL
		CONSTRAINT DF_Infraction_VisibleInClassroom DEFAULT (0)
END
GO

	