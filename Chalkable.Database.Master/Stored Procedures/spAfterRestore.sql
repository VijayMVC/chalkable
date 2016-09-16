Create Procedure spAfterRestore
	@districtId uniqueidentifier
as
	declare @log table
	(
		Id int not null primary key identity,
		Msg nvarchar(2048)
	)	
	
	select * from @log