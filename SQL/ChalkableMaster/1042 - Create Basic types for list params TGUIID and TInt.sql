Create Type TGUID As Table 
(
	[value] [uniqueidentifier] NULL
)
GO

Alter Procedure [dbo].[spCreateLoginInfos]
	@list as TGUID readonly,
	@auto bit
as

if @auto = 0
Begin
	Insert into 
		UserLoginInfo
		(Id) 
	select 
		[value]
	from 
		@List
End
Else Begin
	Insert into 
		UserLoginInfo
		(Id)
	select 
		Id
	from 
		[User]
	where Id not in (select id from UserLoginInfo)
End

GO


Drop Type TIdList
GO

Create Type TInt As Table
(
	[Value] int NULL
)
GO
