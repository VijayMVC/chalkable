CREATE TABLE [dbo].[PersonEthnicity]
(
	[PersonRef]    INT NOT NULL,
	[EthnicityRef] INT NOT NULL,
	[Percentage]   INT NOT NULL,
	[IsPrimary]    BIT NOT NULL,
	CONSTRAINT PK_PersonEthnicity PRIMARY KEY (PersonRef, EthnicityRef),
	CONSTRAINT FK_PersonEthnicity_Person    FOREIGN KEY (PersonRef)    REFERENCES  [Person](Id),
	CONSTRAINT FK_PersonEthnicity_Ethnicity FOREIGN KEY (EthnicityRef) REFERENCES  [Ethnicity](Id)
)
