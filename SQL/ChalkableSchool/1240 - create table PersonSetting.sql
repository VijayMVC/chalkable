CREATE TABLE [dbo].[PersonSetting](
	[PersonRef] [int] NOT NULL,
	[Key] [nvarchar](256) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_PersonSetting_PersonKey] PRIMARY KEY CLUSTERED 
(
	[PersonRef] ASC,
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO

ALTER TABLE [dbo].[PersonSetting]  WITH CHECK ADD  CONSTRAINT [FK_PersonSetting_Person] FOREIGN KEY([PersonRef])
REFERENCES [dbo].[Person] ([Id])
GO

ALTER TABLE [dbo].[PersonSetting] CHECK CONSTRAINT [FK_PersonSetting_Person]
GO


