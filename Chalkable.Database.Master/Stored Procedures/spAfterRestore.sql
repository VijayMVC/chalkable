Create Procedure spAfterRestore
	@districtId uniqueidentifier
as
	declare @log table
	(
		Id int not null primary key identity,
		Msg nvarchar(2048)
	)	
	
	Delete from ApplicationRating where UserRef not in (select id from [User])

	Insert into @log
		(Msg)
	values
		('FUNDS')

	Insert into @log
	(Msg)
	select 
		' PerformedDateTime=' + cast(PerformedDateTime as nvarchar(256))
		+ ' Amount=' + cast(Amount as nvarchar(256))
		+ ' Description=' + Description
		+ ' FromSchoolRef=' + cast(FromSchoolRef as nvarchar(256))

		+ ' ToSchoolRef=' + cast(ToSchoolRef as nvarchar(256))
		+ ' FromUserRef=' + cast(FromUserRef as nvarchar(256))
		+ ' ToUserRef=' + cast(ToUserRef as nvarchar(256))

		+ ' AppInstallActionRef=' + cast(AppInstallActionRef as nvarchar(256))
		+ ' IsPrivate=' + cast(IsPrivate as nvarchar(256))
		+ ' FundRequestRef=' + cast(FundRequestRef as nvarchar(256))
		+ ' SchoolRef=' + cast(SchoolRef as nvarchar(256)) as Msg
	from Fund where 
		ToSchoolRef not in (select id from school)
		or FromSchoolRef not in (select id from school)
		or SchoolRef not in (select id from [School])
		or ToUserRef not in (select id from [user])
		or FromUserRef not in (select id from [user])		
		or FundRequestRef not in (select id from [FundRequest])
		
	Delete from Fund where 
		ToSchoolRef not in (select id from school)
		or FromSchoolRef not in (select id from school)
		or SchoolRef not in (select id from [School])
		or ToUserRef not in (select id from [user])
		or FromUserRef not in (select id from [user])		
		or FundRequestRef not in (select id from [FundRequest])

	Insert into @log
		(Msg)
	values
		('FUND REQUESTS')	

	Insert into @log
		(Msg)
	select 
		' UserRef=' + cast(UserRef as nvarchar(256))
		+ ' CreatedByRef=' + cast(CreatedByRef as nvarchar(256))
		+ ' Amount=' + cast(Amount as nvarchar(256))

		+ ' Created=' + cast(Created as nvarchar(256))
		+ ' PurchaseOrder=' + cast(PurchaseOrder as nvarchar(256))
		+ ' State=' + cast([State] as nvarchar(256))

		+ ' SignatureRef=' + cast(SignatureRef as nvarchar(256))
		+ ' SchoolRef=' + cast(SchoolRef as nvarchar(256)) as Msg
	from 
		FundRequest
	where 
		UserRef not in (select id from [user])		
		or CreatedByRef not in (select id from [user])
		or SchoolRef not in (select id from [School])

	Delete from 
		FundRequest
	where 
		UserRef not in (select id from [user])		
		or CreatedByRef not in (select id from [user])
		or SchoolRef not in (select id from [School])



	Insert into @log
		(Msg)
	values
		('FUND REQUEST ROLE DISTRIBUTIONS')	

	Insert into @log
		(Msg)
	select 
		' RoleRef=' + cast(RoleRef as nvarchar(256))
		+ ' FundRequestRef=' + cast(FundRequestRef as nvarchar(256))
		+ ' Amount=' + cast(Amount as nvarchar(256))
	from 
		FundRequestRoleDistribution
	where
		FundRequestRef not in (select id from FundRequest)

	delete from 
		FundRequestRoleDistribution
	where
		FundRequestRef not in (select id from FundRequest)		

	ALTER TABLE ApplicationRating CHECK CONSTRAINT ALL 
	ALTER TABLE Fund CHECK CONSTRAINT ALL
	ALTER TABLE FundRequest CHECK CONSTRAINT ALL
	ALTER TABLE FundRequestRoleDistribution CHECK CONSTRAINT ALL
	
	select * from @log