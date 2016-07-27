CREATE TABLE [dbo].[PersonNationality]
(
	[Id] INT NOT NULL, 
    [PersonRef] INT NOT NULL, 
    [Nationality] NVARCHAR(128) NOT NULL, 
    [IsPrimary] BIT NOT NULL, 
    [CountryRef] INT NOT NULL,
	CONSTRAINT PK_PersonNationality PRIMARY KEY(Id),
	CONSTRAINT FK_PersonNationality_Person FOREIGN KEY(PersonRef) REFERENCES Person(Id),
	CONSTRAINT FK_PersonNationality_Country FOREIGN KEY(CountryRef) REFERENCES Country(Id)
)