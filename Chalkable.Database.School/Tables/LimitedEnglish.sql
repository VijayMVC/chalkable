Create Table LimitedEnglish
(
	Id int,
    [Code] VARCHAR(5) NOT NULL,
    [Name] VARCHAR(50) NOT NULL,
    [Description] VARCHAR(255) NOT NULL
        CONSTRAINT [DF_LimitedEnglish_Description] DEFAULT (''),
    [StateCode] VARCHAR(10) NOT NULL
        CONSTRAINT [DF_LimitedEnglish_StateCode] DEFAULT(''),
    [SifCode] VARCHAR(10) NOT NULL
        CONSTRAINT [DF_LimitedEnglish_SIFCode] DEFAULT(''),
    [NcesCode] VARCHAR(10) NOT NULL
        CONSTRAINT [DF_LimitedEnglish_NCESCode] DEFAULT(''),
    [IsActive] BIT NOT NULL
        CONSTRAINT [DF_LimitedEnglish_IsActive] DEFAULT (1),
    [IsSystem] BIT NOT NULL
        CONSTRAINT [DF_LimitedEnglish_IsSystem] DEFAULT (0),

	Constraint PK_LimitedEnglish Primary Key(id)
)
