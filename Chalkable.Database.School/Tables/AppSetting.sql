CREATE TABLE [dbo].[AppSetting]
(
	[Name]		VARCHAR(255),
    [Value]		VARCHAR(MAX),
    CONSTRAINT CPK_AppSetting PRIMARY KEY CLUSTERED ([Name])
)