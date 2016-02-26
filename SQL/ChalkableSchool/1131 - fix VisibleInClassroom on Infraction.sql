ALTER TABLE dbo.Infraction
	DROP CONSTRAINT DF_Infraction_VisibleInClassroom
GO

ALTER TABLE dbo.Infraction
	ALTER COLUMN VisibleInClassroom BIT NULL
GO

UPDATE dbo.Infraction 
SET VisibleInClassroom = 0
GO

ALTER TABLE dbo.Infraction
	ALTER COLUMN VisibleInClassroom BIT NOT NULL
GO

	