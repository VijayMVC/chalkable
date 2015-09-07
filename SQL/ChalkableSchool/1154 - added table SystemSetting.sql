CREATE table [dbo].[SystemSetting]
(
 [Category] [nvarchar](25) NOT NULL,
 [Setting] [nvarchar](50) NOT NULL,
 [Value] [nvarchar](max) NOT NULL,

 CONSTRAINT PK_SYSTEM_SETTING PRIMARY KEY CLUSTERED ( [Category] ASC,[Setting] ASC)
) 

