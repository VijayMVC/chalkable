Create Procedure spAfterRestore
	@districtId uniqueidentifier
as
	declare @log table
	(
		Id int not null primary key identity,
		Msg nvarchar(2048)
	)	
	
	Delete from ApplicationRating where UserRef not in (select id from [User])

	ALTER TABLE ApplicationRating CHECK CONSTRAINT ALL
	
	select * from @log