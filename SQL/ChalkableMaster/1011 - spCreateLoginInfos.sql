Create procedure spCreateLoginInfos
as
	Insert into UserLoginInfo
	(Id)
	select 
		Id
	from [User]
	where Id not in (select id from UserLoginInfo)

GO