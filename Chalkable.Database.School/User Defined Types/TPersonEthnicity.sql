CREATE TYPE [dbo].[TPersonEthnicity] AS TABLE
(
	[PersonRef]    INT NOT NULL,
	[EthnicityRef] INT NOT NULL,
	[Percentage]   INT NOT NULL,
	[IsPrimary]    BIT NOT NULL
)
