Drop Type [TPhone]
CREATE TYPE [TPhone] AS TABLE(
	[PersonRef] [int] NOT NULL,
	[Value] [nvarchar](256) NOT NULL,
	[Type] [int] NOT NULL,
	[DigitOnlyValue] [nvarchar](256) NOT NULL,
	[IsPrimary] [bit] NOT NULL	
)
GO


