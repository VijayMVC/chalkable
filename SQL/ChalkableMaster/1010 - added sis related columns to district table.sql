delete from District

GO

ALTER TABLE [dbo].[District]
	ADD [SisUrl] [nvarchar](256) NOT NULL,
		[DbName] [nvarchar](256) NOT NULL,
		[SisUserName] [nvarchar](256) NOT NULL,
		[SisPassword] [nvarchar](256) NOT NULL,
		[SisSystemType] [int] NOT NULL

GO