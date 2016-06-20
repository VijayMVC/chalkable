CREATE TABLE [dbo].[PersonLanguage]
(
	[PersonRef] INT NOT NULL, 
    [LanguageRef] INT NOT NULL, 
    [IsPrimary] BIT NOT NULL DEFAULT 0,
	CONSTRAINT PK_PersonLanguage		   PRIMARY KEY (PersonRef, LanguageRef),
	CONSTRAINT FK_PersonLanguage_Person    FOREIGN KEY (PersonRef)    REFERENCES  [Person](Id),
	CONSTRAINT FK_PersonLanguage_Language  FOREIGN KEY (LanguageRef)  REFERENCES  [Ethnicity](Id)
)

/**/