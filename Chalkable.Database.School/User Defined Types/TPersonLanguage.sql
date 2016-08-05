CREATE TYPE [dbo].[TPersonLanguage] AS TABLE
(
	[PersonRef] INT, 
    [LanguageRef] INT, 
    [IsPrimary] BIT
)