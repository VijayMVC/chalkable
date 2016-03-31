CREATE TYPE [dbo].[TSystemSetting] AS TABLE (
 [Category] [nvarchar](25) NOT NULL,
 [Setting] [nvarchar](50) NOT NULL,
 [Value] [nvarchar](max) NOT NULL,
 PRIMARY KEY CLUSTERED 
 (
	 [Category] ASC,
	[Setting] ASC
 )
) 
