Create Type TIdList As Table
(
	[Id] [uniqueidentifier] NULL
)
GO

Alter Procedure [dbo].[spCreateLoginInfos]
	@list as TIdList readonly,
	@auto bit
as

if @auto = 0
Begin
	Insert into 
		UserLoginInfo
		(Id) 
	select 
		Id 
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


