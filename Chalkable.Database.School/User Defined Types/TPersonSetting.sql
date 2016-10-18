Create Type [dbo].[TPersonSetting] As Table(
	[Id] INT NOT NULL,
    [PersonRef]     INT NULL,
    [SchoolYearRef] INT  NULL,
	[ClassRef]	INT	 NULL,
    [Key]  NVARCHAR (256) NOT NULL,
    [Value] NVARCHAR (MAX) NULL
)